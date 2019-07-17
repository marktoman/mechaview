using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Mecha.View;
using Mecha.View.Wpf.Converters;
using Mecha.ViewModel;
using Mecha.ViewModel.Attributes;
using Ctrl = System.Windows.Controls;
using MechaCtrl = Mecha.View.Wpf.Controls;
using VM = Mecha.ViewModel.Elements;
using Win = System.Windows;

namespace Mecha.View.Wpf
{
    internal class ControlBuilder
    {
        readonly Dialogs dialogs;
        public ControlBuilder(Dialogs dialogs)
        {
            this.dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
        }
        
        public MechaCtrl.MainPanel CreateControls(IEnumerable<VM.Element> elements)
        {
            var groups = elements
                .GroupBy(x => x.Attribute?.Group)
                .ToArray();
         
            if (groups.Length == 1 && groups.First().Key == null)
            {
                var mp = new MechaCtrl.MainPanel();

                var ctrls = CreateStatesAndActions(groups.First());
                foreach (var x in ctrls)
                    mp.Children.Add(x);

                return mp;
            }

            var gt = groups.First(x => x.Key != null).Key.GetType();
            var names = Enum.GetNames(gt);
            
            var namedGroups = names
                .Join(groups, x => x, x => x.Key?.ToString(), (x, y) => y)
                .ToArray();

            var mainPanel = new MechaCtrl.MainPanel();

            if (namedGroups.Length > 0)
                mainPanel.HasRootPanelControls = true;

            foreach (var g in namedGroups)
            {
                var groupPanel = new MechaCtrl.GroupPanel
                {
                    Margin = new Win.Thickness(0, 15, 0, 15),
                    Orientation = Ctrl.Orientation.Vertical,
                    Height = double.NaN,
                };
                var csaa = CreateStatesAndActions(g);
                foreach (var x in csaa)
                    groupPanel.Children.Add(x);

                string name = g.Key.ToString();
                var ga = gt.GetMember(name).FirstOrDefault()?.GetCustomAttribute<GroupAttribute>();

                if (ga?.Invisible != true)
                {
                    string header = ga?.DisplayName
                        ?? Mecha.Helpers.Utils.NameToDisplayName(name);

                    var gpBox = new Ctrl.GroupBox
                    {
                        Name = name,
                        Header = header,
                        Content = groupPanel,
                    };

                    mainPanel.Children.Add(gpBox);
                }
                else
                {
                    mainPanel.Children.Add(groupPanel);
                }
            }

            foreach (var g in groups.Except(namedGroups))
            {
                var csaa = CreateStatesAndActions(g);
                foreach (var x in csaa)
                    mainPanel.Children.Add(x);
            }

            return mainPanel;
        }
        IEnumerable<Win.UIElement> CreateStatesAndActions(IEnumerable<VM.Element> childElements)
        {
            var rows = childElements.GroupBy(x =>
            {
                var val = x?.Attribute?.Position;
                return val == null || double.IsNaN(val.Value) || val < 0
                    ? -1
                    : (int) x.Attribute.Position;
            });

            var autoRow = rows.SingleOrDefault(x => x.Key == -1);
            if (autoRow != null)
                rows = rows.Where(x => x.Key >= 0);
            rows = rows.OrderBy(x => x.Key);

            foreach (var row in rows)
            {
                var elements = row.OrderBy(x =>
                {
                    double k = x.Attribute.Position;
                    k -= (int)k;
                    k *= 10;
                    return (int)k;

                }).ToArray();

                if (elements.Length > 1)
                {
                    var rowGrid = new MechaCtrl.RowGrid
                    {
                        Width = double.NaN,
                        HorizontalAlignment = Win.HorizontalAlignment.Stretch,
                    };

                    var exactWidthElements = elements.Where(x => x.Attribute?.Width > 0).ToArray();
                    var autoWidthElements = elements.Except(exactWidthElements).ToArray();
                    int totalExactWidth = exactWidthElements.Sum(x => x.Attribute.Width);

                    if (totalExactWidth > 100)
                        throw new Exception("The total width cannot exceed 100.");
                    if (totalExactWidth == 100 && autoWidthElements.Length > 0)
                        throw new Exception("The total width equals to 100 while not all elements have been included.");

                    int autoWidth = autoWidthElements.Length > 0
                        ? (100 - totalExactWidth) / autoWidthElements.Length
                        : -1;

                    //TODO: Add fixed-width element support (button etc.)

                    int i = 0;
                    foreach (var e in elements)
                    {
                        int width = e.Attribute?.Width > 0 ? e.Attribute.Width : autoWidth;
                        rowGrid.ColumnDefinitions.Add(new Ctrl.ColumnDefinition
                        {
                            Width = new Win.GridLength(width, Win.GridUnitType.Star)
                        });

                        var ctrl = ConvertElement(e);
                        Ctrl.Grid.SetColumn(ctrl, i);

                        rowGrid.Children.Add(ctrl);

                        i++;
                    }
                     
                    yield return rowGrid;                    
                }
                else
                {
                    yield return ConvertElement(elements.Single());
                }
            }

            if (autoRow != null)
            {
                var buttons = autoRow.OfType<VM.Button>().ToArray();
                var states = autoRow.Except(buttons);

                foreach (var x in states)
                    yield return ConvertElement(x);

                if (buttons.Length > 0)
                {
                    var btnPanel = new MechaCtrl.ActionPanel();

                    foreach (var x in buttons)
                        btnPanel.Children.Add(ConvertElement(x));

                    yield return btnPanel;
                }
            }
        }

        Ctrl.Control ConvertElement(VM.Element vm)
        {
            var t = vm.GetType();

            if (t == typeof(VM.TableView))
            {
                var tv = (VM.TableView)vm;
                
                var lv = new Ctrl.ListView { DataContext = tv };
                lv.SetBinding(Ctrl.ListView.ItemsSourceProperty, tv.Path);

                lv.SetBinding(Ctrl.ListView.VisibilityProperty, new Binding("Items.Count")
                {
                    RelativeSource = RelativeSource.Self,
                    Mode = BindingMode.OneWay,
                    Converter = new IntToVisibilityConverter()
                });

                if (tv.ItemType.IsPrimitive != true)
                {
                    var gv = new Ctrl.GridView();

                    var props = tv.ItemType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var xt in props)
                    {
                        gv.Columns.Add(new Ctrl.GridViewColumn
                        {
                            DisplayMemberBinding = new Binding(xt.Name),
                            Header = xt.Name,
                            Width = Double.NaN,
                        });
                    }
                    lv.View = gv;
                }

                return lv;
            }

            if (t == typeof(VM.SlideInput))
            {
                var slider = new Ctrl.Slider
                {
                    Name = vm.Name,
                    DataContext = (VM.SlideInput)vm,
                };
                slider.SetBinding(Ctrl.Slider.ValueProperty, vm.Path);
                return slider;
            }

            if (t == typeof(VM.EnumInput))
            {
                return new MechaCtrl.EnumInput
                {
                    Name = vm.Name,
                    ViewModel = (VM.EnumInput)vm
                };
            }

            if (t == typeof(VM.TextView))
            {
                return new MechaCtrl.TextView
                {
                    Name = vm.Name,
                    ViewModel = (VM.TextView)vm,
                };
            }

            if (t == typeof(VM.CheckBox))
            {
                var chkBox = new Ctrl.CheckBox
                {
                    Name = vm.Name,
                    DataContext = (VM.CheckBox)vm,
                };
                chkBox.SetBinding(Ctrl.CheckBox.IsCheckedProperty, vm.Path);
                return chkBox;
            }

            if (t == typeof(VM.DateInput))
            {
                return new MechaCtrl.DateInput
                {
                    Name = vm.Name,
                    ViewModel = (VM.DateInput)vm,
                };
            }

            if (t == typeof(VM.TextInput))
            {
                return new MechaCtrl.TextInput
                {
                    Name = vm.Name,
                    ViewModel = (VM.TextInput)vm,
                };
            }

            if (t == typeof(VM.PathInput))
            {
                return new MechaCtrl.PathInput
                {
                    Name = vm.Name,
                    ViewModel = (VM.PathInput)vm,
                };
            }

            if (t == typeof(VM.PasswordInput))
            {
                return new MechaCtrl.PasswordInput
                {
                    Name = vm.Name,
                    ViewModel = (VM.PasswordInput)vm,
                };
            }

            if (t == typeof(VM.Button))
            {
                var btn = new Ctrl.Button
                {
                    Name = vm.Name,
                    DataContext = vm,
                };
                var btnVm = (VM.Button)vm;
                btn.Click += async (o, e) => await TryInvokeButton(btnVm);
                return btn;
            }

            throw new NotImplementedException();
        }

        async Task TryInvokeButton(VM.Button vm)
        {
            if (vm.ActionAttributes?.Any(x => x.DisableValidation) != true)
            {
                bool hasErr = false;
                foreach (var x in vm.Parent.Elements.OfType<VM.StateElement>())
                {
                    if (!x.Validate())
                        hasErr = true;
                }
                if (hasErr)
                    return;
            }

            var confirmAttr = vm?.ActionAttributes.OfType<ConfirmAttribute>().SingleOrDefault();
            if (confirmAttr != null)
            {
                var proceed = await dialogs
                    .ShowConfirm(
                        confirmAttr.Title,
                        confirmAttr.Message,
                        confirmAttr.AffirmativeText ?? "OK",
                        confirmAttr.DismissiveText ?? "Cancel");

                if (proceed)
                    await InvokeButtonAction(vm);
            }
            else
            {
                await InvokeButtonAction(vm);
            }
        }

        async Task InvokeButtonAction(VM.Button vm)
        {
            var msgAttr = vm?.ActionAttributes.OfType<MessageAttribute>().SingleOrDefault();
            var progAttr = vm?.ActionAttributes.OfType<ProgressAttribute>().SingleOrDefault();

            var pars = vm.Method.GetParameters();

            var hasProgPar = pars.SingleOrDefault(x => x.ParameterType == typeof(IMechaProgress)) != null;
            var hasMsgPar = pars.SingleOrDefault(x => x.ParameterType == typeof(IMechaMessage)) != null;
            var hasActionPar = pars.SingleOrDefault(x => x.ParameterType == typeof(IMechaAction)) != null;

            var hasProg = hasProgPar || progAttr != null;
            var hasMsg = hasMsgPar || msgAttr != null;

            IMechaMessage msgDial = hasMsgPar ? new MessageDialog() : null;

            async Task InvokeAction(IMechaProgress progDial)
            {
                var argl = new List<Tuple<Type, object>>();

                if (hasProgPar)
                    argl.Add(Tuple.Create(typeof(IMechaProgress), (object)progDial));
                if (hasMsgPar)
                    argl.Add(Tuple.Create(typeof(IMechaMessage), (object)msgDial));
                if (hasActionPar)
                    argl.Add(Tuple.Create(typeof(IMechaAction), (object)new ButtonHandle(vm)));

                var args = argl.Count > 0
                    ? pars.Join(argl,
                        x => x.ParameterType,
                        x => x.Item1,
                        (x, y) => y.Item2).ToArray()
                    : null;

                if (argl.Count > 0 && argl.Count != args.Length)
                    throw new Exception($"The '{vm.Method.Name}' method has unsupported parameters.");

                if (vm.Method.ReturnType == typeof(Task))
                    await (Task)vm.Invoke(args);
                else
                    vm.Invoke(args);
            }

            bool actionError = false;

            if (hasProg)
            {
                var message = progAttr?.Message ?? "Please wait...";
                var title = progAttr?.Title ?? "processing";

                var progDial = await dialogs.ShowProgress(title, message, false);

                try
                {
                    await InvokeAction(progDial);
                }
                catch (Exception exception)
                {
                    actionError = true;

                    if (progDial.IsOpen)
                        await progDial.CloseAsync();

                    await dialogs.ShowError("Error", exception.GetBaseException().Message);
                }

                if (progDial.IsOpen)
                    await progDial.CloseAsync();
            }
            else
            {
                try
                {
                    await InvokeAction(null);
                }
                catch (Exception exception)
                {
                    actionError = true;
                    await dialogs.ShowError("Error", exception.GetBaseException().Message);
                }
            }

            if (hasMsg)
            {
                var message = msgAttr?.Message ?? "Finished.";
                var title = msgAttr?.Title ?? "done";

                if (!actionError)
                    await dialogs.ShowInfo(msgDial?.Title ?? title, msgDial?.Message ?? message);
            }
        }
    }
}
