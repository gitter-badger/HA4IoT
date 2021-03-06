﻿using System;
using System.Collections.Generic;
using System.Linq;
using HA4IoT.ManagementConsole.Configuration.ViewModels;
using HA4IoT.ManagementConsole.Configuration.ViewModels.Settings;
using HA4IoT.ManagementConsole.Json;
using Newtonsoft.Json.Linq;

namespace HA4IoT.ManagementConsole.Configuration
{
    public class ActuatorParser
    {
        private readonly JProperty _source;

        private JObject _root;
        private JObject _settings;
        private JObject _appSettings;

        private string _type;

        public ActuatorParser(JProperty source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            _source = source;
        }

        public ActuatorItemVM Parse()
        {
            _type = _source.Value["type"].Value<string>();

            _root = (JObject)_source.Value;
            _settings = (JObject)_source.Value["settings"];
            _appSettings = _settings.GetNamedObject("appSettings", new JObject());

            var item = new ActuatorItemVM(_source.Name, _type);
            item.SortValue = (int)_appSettings.GetNamedNumber("SortValue", 0);

            item.Settings.AddRange(GenerateGeneralSettings());

            item.IsEnabled = (BoolSettingVM)item.Settings.First(s => s.Key == "IsEnabled");
            item.Caption = (StringSettingVM)item.Settings.First(s => s.Key == "Caption");

            switch (_type)
            {
                case "RollerShutter":
                    {
                        item.Settings.AddRange(GenerateRollerShutterSettings());
                        break;
                    }

                case "StateMachine":
                    {
                        item.Settings.AddRange(GenerateStateMachineSettings());
                        item.Settings.AddRange(GenerateOnStateCounterSettings());
                        break;
                    }

                case "Lamp":
                    {
                        item.Settings.AddRange(GenerateOnStateCounterSettings());
                        break;
                    }

                case "Socket":
                    {
                        item.Settings.AddRange(GenerateOnStateCounterSettings());
                        break;
                    }

                case "HumiditySensor":
                    {
                        item.Settings.AddRange(GenerateHumiditySensorSettings());
                        break;
                    }

                case "VirtualButtonGroup":
                    {
                        item.Settings.AddRange(GenerateVirtualButtonGroupSettings());
                        break;
                    }
            }

            return item;
        }

        private IEnumerable<SettingItemVM> GenerateHumiditySensorSettings()
        {
            yield return FloatSettingVM.CreateFrom(_appSettings, "WarningValue", 60, "Warning value");
            yield return FloatSettingVM.CreateFrom(_appSettings, "DangerValue", 70, "Danger value");
        } 

        private IEnumerable<SettingItemVM> GenerateOnStateCounterSettings()
        {
            yield return
                BoolSettingVM.CreateFrom(_appSettings, "IsPartOfOnStateCounter", false, "Is part of 'On-State' counter");

            yield return StringSettingVM.CreateFrom(_appSettings, "OnStateId", "On", "'On-State' ID");
        }

        private IEnumerable<SettingItemVM> GenerateGeneralSettings()
        {
            yield return new StringSettingVM("Id", _source.Name, "ID") { IsReadOnly = true };
            yield return new StringSettingVM("type", _type, "Type") { IsReadOnly = true };
            yield return BoolSettingVM.CreateFrom(_settings, "IsEnabled", true, "Enabled").WithIsNoAppSetting();
            yield return BoolSettingVM.CreateFrom(_appSettings, "Hide", false, "Hidden");
            yield return StringSettingVM.CreateFrom(_appSettings, "Image", "DefaultActuator", "Image");
            yield return StringSettingVM.CreateFrom(_appSettings, "Caption", _source.Name, "Caption");
            yield return StringSettingVM.CreateFrom(_appSettings, "OverviewCaption", _source.Name, "Caption (Overviews)");
        }

        private IEnumerable<SettingItemVM> GenerateStateMachineSettings()
        {
            yield return BoolSettingVM.CreateFrom(_appSettings, "DisplayVertical", false, "Display vertical");

            foreach (var state in _root.GetNamedArray("supportedStates"))
            {
                string key = $"Caption.{state}";
                yield return new StringSettingVM(key, _appSettings.GetNamedString(key, key), $"Caption for '{state}'");
            }
        }

        private IEnumerable<SettingItemVM> GenerateVirtualButtonGroupSettings()
        {
            yield return BoolSettingVM.CreateFrom(_appSettings, "DisplayVertical", false, "Display vertical");

            foreach (var state in _root.GetNamedArray("buttons"))
            {
                string key = $"Caption.{state}";
                yield return new StringSettingVM(key, _appSettings.GetNamedString(key, key), $"Caption for '{state}'");
            }
        }

        private IEnumerable<SettingItemVM> GenerateRollerShutterSettings()
        {
            yield return IntSettingVM.CreateFrom(_appSettings, "MaxPosition", 20000, "Max position");
            yield return TimeSpanSettingVM.CreateFrom(_settings, "AutoOffTimeout", TimeSpan.FromSeconds(22), "Auto off");
        }
    }
}
