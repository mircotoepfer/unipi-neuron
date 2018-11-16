// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

using adcoso.iot.devicedrivers.neuron.driver;
using adcoso.iot.devicedrivers.neuron.driver.analoginput;
using adcoso.iot.devicedrivers.neuron.driver.analogoutput;
using adcoso.iot.devicedrivers.neuron.driver.board;
using adcoso.iot.devicedrivers.neuron.driver.digitalinputs;
using adcoso.iot.devicedrivers.neuron.driver.digitaloutputs;
using adcoso.iot.devicedrivers.neuron.driver.onewire.interfaces;
using adcoso.iot.devicedrivers.neuron.driver.userled;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace adcoso.iot.devicedrivers.neuron.application
{

    internal delegate void ActionDelegate();

    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public sealed partial class MainPage
    {
        
        private readonly NeuronDevice _device;

        private event ActionDelegate AllLedsOff;
        private event ActionDelegate AllLedsOn;
        private event ActionDelegate AllLedsBlink;

        public MainPage()
        {
            InitializeComponent();

            _device = new NeuronDevice();
            _device.OnInitializationFinish += Device_OnInitializationFinish;
            _device.Initialize().ConfigureAwait(false);

        }

        private void Device_OnInitializationFinish()
        {
            SetupBoardInformations(_device.BoardInformations);
            SetupDigitalInputs(_device.DigitalInputs);
            SetupUserLeds(_device.UserLeds);
            SetupOutputs(_device.DigitalOutputs);
            SetupAnalogOutputs(_device.AnalogOutputs);
            SetupAnalogInputs(_device.AnalogInputs);
            SetupOneWire(_device.OneWireConnectors);
        }

        private void SetupOneWire(IReadOnlyList<IOneWireConnector> deviceOneWireConnectors)
        {
            Grid inputGrid = null;

            int row = 0;

            foreach (IOneWireConnector oneWireConnector in deviceOneWireConnectors)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                }

                inputGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var control = new OneWireControl(oneWireConnector);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, 0);
                Grid.SetRow(control, row++);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "One Wire Connectors", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }
        }

        private void SetupDigitalInputs(IReadOnlyList<IDigitalInput> deviceDigitalInputs)
        {
            Grid inputGrid = null;

            int column = 0;

            foreach (IDigitalInput deviceDigitalInput in deviceDigitalInputs)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.RowDefinitions.Add(new RowDefinition());
                }

                inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var control = new DigitalInputControl(deviceDigitalInput);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, column++);
                Grid.SetRow(control, 1);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "Digital Inputs", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }

        }
        private void SetupBoardInformations(IReadOnlyList<IBoardInformation> boardInformations)
        {
            Grid inputGrid = null;

            int column = 0;

            foreach (IBoardInformation boardInformation in boardInformations)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.RowDefinitions.Add(new RowDefinition());
                }

                inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var control = new HardwareInformationControl(boardInformation);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, column++);
                Grid.SetRow(control, 1);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "Board Information", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }

        }
        private void SetupOutputs(IReadOnlyList<IDigitalOutput> deviceDigitalOutputs)
        {
            Grid inputGrid = null;

            int column = 1;

            foreach (var digitalOutput in deviceDigitalOutputs)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.RowDefinitions.Add(new RowDefinition());
                    inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var generalPanel = new StackPanel();
                    generalPanel.Width = 110;
                    generalPanel.Margin = new Thickness(5);
                    generalPanel.BorderBrush = new SolidColorBrush(Colors.Green);
                    generalPanel.BorderThickness = new Thickness(1);

                    var allOn = new Button();
                    allOn.Content = "All On";
                    allOn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    allOn.Margin = new Thickness(5);
                    allOn.Click += AllOn_Click;

                    generalPanel.Children.Add(allOn);

                    var allOff = new Button();
                    allOff.Content = "All Off";
                    allOff.HorizontalAlignment = HorizontalAlignment.Stretch;
                    allOff.Margin = new Thickness(5);
                    allOff.Click += AllOffOnClick;

                    generalPanel.Children.Add(allOff);

                    var toggle = new Button();
                    toggle.Content = "Toggle All";
                    toggle.HorizontalAlignment = HorizontalAlignment.Stretch;
                    toggle.Margin = new Thickness(5);
                    toggle.Click += ToggleOnClick;

                    generalPanel.Children.Add(toggle);

                    inputGrid.Children.Add(generalPanel);
                    Grid.SetColumn(generalPanel, 0);

                }

                inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var control = new DigitalOutputControl(digitalOutput);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, column++);
                Grid.SetRow(control, 1);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "Digital Outputs", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }
        }
        private void SetupAnalogOutputs(IReadOnlyList<IAnalogOutput> analogOutputs)
        {
            Grid inputGrid = null;

            int column = 0;

            foreach (var analogOutput in analogOutputs)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.RowDefinitions.Add(new RowDefinition());
                }

                inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var control = new AnalogOutputControl(analogOutput);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, column++);
                Grid.SetRow(control, 1);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "Analog Outputs", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }
        }

        private void SetupUserLeds(IReadOnlyList<IUserLed> userLeds)
        {
            Grid inputGrid = null;

            int column = 1;

            foreach (var userLed in userLeds)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.RowDefinitions.Add(new RowDefinition());
                    inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                    var generalPanel = new StackPanel();
                    generalPanel.Width = 110;
                    generalPanel.Margin = new Thickness(5);
                    generalPanel.BorderBrush = new SolidColorBrush(Colors.Green);
                    generalPanel.BorderThickness = new Thickness(1);

                    var allOn = new Button();
                    allOn.Content = "All On";
                    allOn.HorizontalAlignment = HorizontalAlignment.Stretch;
                    allOn.Margin = new Thickness(5);
                    allOn.Click += AllLedOn_Click;

                    generalPanel.Children.Add(allOn);

                    var allOff = new Button();
                    allOff.Content = "All Off";
                    allOff.HorizontalAlignment = HorizontalAlignment.Stretch;
                    allOff.Margin = new Thickness(5);
                    allOff.Click += AllLedOffOnClick;

                    generalPanel.Children.Add(allOff);

                    var allBlink = new Button();
                    allBlink.Content = "All Blink";
                    allBlink.HorizontalAlignment = HorizontalAlignment.Stretch;
                    allBlink.Margin = new Thickness(5);
                    allBlink.Click += AllLedBlink_Click;

                    generalPanel.Children.Add(allBlink);

                    inputGrid.Children.Add(generalPanel);
                    Grid.SetColumn(generalPanel, 0);

                }

                inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var control = new UserLedControl(userLed);

                this.AllLedsOff += () => control.ViewModel.OffCommand.Execute(null);
                this.AllLedsOn += () => control.ViewModel.OnCommand.Execute(null);
                this.AllLedsBlink += () => control.ViewModel.BlinkCommand.Execute(null);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, column++);
                Grid.SetRow(control, 1);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "User LED's", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }
        }

        private void SetupAnalogInputs(IReadOnlyList<IAnalogInput> analogInputs)
        {
            Grid inputGrid = null;

            int column = 0;

            foreach (IAnalogInput analogInput in analogInputs)
            {
                if (inputGrid == null)
                {
                    inputGrid = new Grid();
                    inputGrid.RowDefinitions.Add(new RowDefinition());
                }

                inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var control = new AnalogInputControl(analogInput);

                inputGrid.Children.Add(control);
                Grid.SetColumn(control, column++);
                Grid.SetRow(control, 1);

            }

            if (inputGrid != null)
            {
                ContentPanel.Children.Add(new TextBlock() { Text = "Analog Inputs", Margin = new Thickness(5) });
                ContentPanel.Children.Add(inputGrid);
            }

        }


        private void AllLedOffOnClick(object sender, RoutedEventArgs e)
        {

            AllLedsOff?.Invoke();
        }

        private void AllLedOn_Click(object sender, RoutedEventArgs e)
        {

            AllLedsOn?.Invoke();
        }
        private void AllLedBlink_Click(object sender, RoutedEventArgs e)
        {

            AllLedsBlink?.Invoke();
        }


        private void ToggleOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (IDigitalOutput deviceDigitalOutput in _device.DigitalOutputs)
            {
                deviceDigitalOutput.ToggleOutput();
            }
        }

        private void AllOffOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (IDigitalOutput deviceDigitalOutput in _device.DigitalOutputs)
            {
                deviceDigitalOutput.SetOutputValue(driver.commons.OnOffValue.Off);
            }
        }

        private void AllOn_Click(object sender, RoutedEventArgs e)
        {
            foreach (IDigitalOutput deviceDigitalOutput in _device.DigitalOutputs)
            {
                deviceDigitalOutput.SetOutputValue(driver.commons.OnOffValue.On);
            }
        }

    }
}
