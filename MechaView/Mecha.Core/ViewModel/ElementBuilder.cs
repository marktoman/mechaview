using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mecha.ViewModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Mecha.Helpers;
using Mecha.Exceptions;
using Castle.DynamicProxy;
using Mecha.ViewModel.Attributes;
using Mecha.ViewModel.Elements;
using System.Threading.Tasks;

namespace Mecha.ViewModel
{
    internal static class ElementBuilder
    {
        public static Element[] CreateElements(Type type, Container parent)
        {
            //TODO: Add support for custom types (compose FormViewModel recursively)

            var stateElements = CreateStateElements(type, parent);
            var actionElements = CreateActionElements(type, parent);

            return stateElements
                .Cast<Element>()
                .Concat(actionElements)
                .ToArray();
        }

        static IEnumerable<StateElement> CreateStateElements(Type srcType, Container parent)
        {
            var props = srcType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                if (!p.GetMethod.IsVirtual) throw new InvalidOperationException("All public properties have to be marked virtual");
            }

            var readonlyProps = props.Where(IsReadonlyProperty).ToArray();
            var inputProps = props.Except(readonlyProps);

            var readonlyElements = readonlyProps.Select(x => CreateReadonlyStateElement(x, parent));
            var inputElements = inputProps.Select(x => CreateInputStateElement(x, parent));

            return readonlyElements.Concat(inputElements);
        }
        static bool IsReadonlyProperty(PropertyInfo property)
        {
            if (property.GetSetMethod(false) == null ||
                property.GetCustomAttribute<ReadonlyAttribute>(true) != null)
                return true;

            var nameWords = Utils.NameToDisplayName(property.Name).Split(' ');
            if (nameWords.Contains("Label"))
                return true;

            return false;
        }
        static StateElement CreateReadonlyStateElement(PropertyInfo property, Container parent)
        {
            var attr = property.GetCustomAttribute<ElementAttribute>(true);
            return new TextView(parent, property, property.Name)
            {
                Attribute = attr
            };
        }
        static StateElement CreateInputStateElement(PropertyInfo property, Container parent)
        {
            var attr = property.GetCustomAttribute<ElementAttribute>(true);
            string description = attr?.Description;
            var displayName = attr?.DisplayName ?? Utils.NameToDisplayName(property.Name);
            var displayWords = displayName.Split(' ');

            var t = property.PropertyType;
            if (t.IsArray || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var itemType = t.IsArray
                    ? t.GetElementType()
                    : t.GetGenericArguments().Single();

                return new TableView(itemType, parent, property, property.Name)
                {
                    DisplayName = displayName,
                    Description = description,
                    Attribute = attr
                };
            }

            if (t == typeof(bool) && t.IsPublic)
            {
                return new CheckBox(parent, property, property.Name)
                {
                    DisplayName = displayName,
                    Description = description,
                    Attribute = attr,
                };
            }

            if (t.IsEnum)
            {
                return new EnumInput(parent, property, property.Name)
                {
                    DisplayName = displayName,
                    Description = description,
                    Attribute = attr,
                };
            }

            // TODO: Add intermediate state support

            var rangeAttrib = t.GetCustomAttribute<RangeAttribute>(true);

            if (t.IsNumeric() && (
                rangeAttrib != null ||
                displayWords.Contains("Range")))
            {
                double min = rangeAttrib?.Min ?? 0;
                double max = rangeAttrib?.Max ??
                    (t == typeof(int) ? 100 :
                    t == typeof(double) ? 1 :
                    t == typeof(float) ? 1 : 10);

                return new SlideInput(min, max, parent, property, property.Name)
                {
                    DisplayName = displayName,
                    Description = description,
                    Attribute = attr,
                };
            }

            if (attr?.GetType() == typeof(DateInputAttribute) ||
                (t == typeof(DateTime) && property.SetMethod.IsPublic))
            {
                return new DateInput(parent, property, property.Name)
                {
                    DisplayName = displayName,
                    Description = description,
                    Attribute = attr,
                };
            }

            if (attr?.GetType() == typeof(PathAttribute) ||
                (t == typeof(string) && displayWords.Contains("Path") && property.SetMethod.IsPublic))
            {
                if (!property.SetMethod.IsPublic)
                    throw new NotSupportedException("The setter of a Path property has to be public.");

                ElementAttribute elAttr = null;
                var pa = attr as PathAttribute;
                var pathType = PathType.Open;
                if ((pa == null || pa.Type != PathType.Open) && displayWords.Contains("Save"))
                {
                    pathType = PathType.Save;
                }
                else if (pa != null)
                {
                    pathType = pa.Type;
                }

                if (displayWords.Length > 1)
                    displayName = string.Join(" ", displayWords.Where(x => x != "Path"));

                var pathInput = new PathInput(parent, property, property.Name, pathType)
                {
                    DisplayName = displayName,
                    Description = description
                };

                pathInput.Attribute = attr as ElementAttribute;

                return pathInput;
            }

            if (attr?.GetType() == typeof(PasswordAttribute) ||
                (t == typeof(string) && displayWords.Contains("Password") && property.SetMethod.IsPublic))
            {
                return new PasswordInput(parent, property, property.Name)
                {
                    DisplayName = displayName,
                    Description = description,
                    Attribute = attr,
                };
            }

            return (StateElement)new TextInput(parent, property, property.Name)
            {
                DisplayName = displayName,
                Description = description,
                Attribute = attr,
            };
        }
        static IEnumerable<ActionElement> CreateActionElements(Type srcType, Container parent)
        {
            var objectMethods = typeof(object).GetMethods().Select(x => x.Name).ToArray();
            var interceptorMethods = new[] { "DynProxyGetTarget", "DynProxySetTarget", "GetInterceptors" };
            var proxySpecialNames = srcType
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.IsSpecialName)
                .Select(x => x.Name + "_callback")
                .ToArray();

            return srcType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x =>
                    !x.IsSpecialName &&
                    !proxySpecialNames.Contains(x.Name) &&
                    !objectMethods.Contains(x.Name) &&
                    !interceptorMethods.Contains(x.Name))
                .Select(method =>
                {
                    var elementAttrib = method.GetCustomAttribute<ElementAttribute>(true);
                    var displayName = elementAttrib?.DisplayName ?? Utils.NameToDisplayName(method.Name);

                    if (method.IsVirtual) throw new InvalidOperationException("Methods cannot be virtual");
                    if (method.ReturnType != typeof(Task) && method.ReturnType != typeof(void)) throw new InvalidOperationException("Methods have to return either Task or void");

                    return new Button(parent, method, method.Name)
                    {
                        DisplayName = displayName,
                        Attribute = elementAttrib,
                        ActionAttributes = method.GetCustomAttributes<ActionAttribute>(true).ToArray(),
                        Description = elementAttrib?.Description,
                    };
                });
        }
    }
}
