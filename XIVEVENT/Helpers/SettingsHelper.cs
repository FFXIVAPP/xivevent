// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHelper.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SettingsHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Helpers {
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using XIVEVENT.Models;
    using XIVEVENT.ViewModels;

    public static class SettingsHelper {
        public static void SaveChatCodes() {
            IEnumerable<XElement> xElements = AppViewModel.Instance.XChatCodes.Descendants().Elements("Code");
            XElement[] enumerable = xElements as XElement[] ?? xElements.ToArray();

            foreach (ChatCode chatCode in AppViewModel.Instance.ChatCodes) {
                XElement element = enumerable.FirstOrDefault(e => e.Attribute("Key")?.Value == chatCode.Code);

                string xKey = chatCode.Code;
                string xColor = chatCode.Color;
                string xDescription = chatCode.Description;

                List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

                keyValuePairs.Add(new KeyValuePair<string, string>("Color", xColor));
                keyValuePairs.Add(new KeyValuePair<string, string>("Description", xDescription));

                if (element is null) {
                    XMLHelper.SaveXMLNode(AppViewModel.Instance.XChatCodes, "Codes", "Code", xKey, keyValuePairs);
                }
                else {
                    XElement xColorElement = element.Element("Color");
                    if (xColorElement is not null) {
                        xColorElement.Value = xColor;
                    }
                    else {
                        element.Add(new XElement("Color", xColor));
                    }

                    XElement xDescriptionElement = element.Element("Description");
                    if (xDescriptionElement is not null) {
                        xDescriptionElement.Value = xDescription;
                    }
                    else {
                        element.Add(new XElement("Description", xDescription));
                    }
                }
            }

            AppViewModel.Instance.XChatCodes.Save(Path.Combine(AppViewModel.Instance.ConfigurationsPath, "ChatCodes.xml"));
        }

        public static void SaveEvents() {
            AppViewModel.Instance.XEvents.Descendants("Event").Where(node => AppViewModel.Instance.EventItems.All(item => item.Key?.ToString() != node.Attribute("Key")?.Value)).Remove();
            IEnumerable<XElement> xElements = AppViewModel.Instance.XEvents.Descendants().Elements("Event");
            XElement[] enumerable = xElements as XElement[] ?? xElements.ToArray();

            foreach (EventItem eventItem in AppViewModel.Instance.EventItems) {
                XElement element = enumerable.FirstOrDefault(e => e.Attribute("Key")?.Value.ToString() == eventItem.Key.ToString());

                string xKeyString = eventItem.Key.ToString();
                string xIsEnabled = eventItem.IsEnabled.ToString();
                string xRegEx = eventItem.RegEx;
                string xCategory = eventItem.Category;
                string xSound = eventItem.Sound;
                string xVolume = eventItem.Volume.ToString(CultureInfo.InvariantCulture);
                string xSpeechRate = eventItem.SpeechRate.ToString(CultureInfo.InvariantCulture);
                string xTTSMessage = eventItem.TTSMessage;
                string xExecutablePath = eventItem.ExecutablePath;
                string xExecutableArguments = eventItem.ExecutableArguments;

                List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();

                keyValuePairs.Add(new KeyValuePair<string, string>("IsEnabled", xIsEnabled));
                keyValuePairs.Add(new KeyValuePair<string, string>("RegEx", xRegEx));
                keyValuePairs.Add(new KeyValuePair<string, string>("Category", xCategory));
                keyValuePairs.Add(new KeyValuePair<string, string>("Sound", xSound));
                keyValuePairs.Add(new KeyValuePair<string, string>("Volume", xVolume));
                keyValuePairs.Add(new KeyValuePair<string, string>("SpeechRate", xSpeechRate));
                keyValuePairs.Add(new KeyValuePair<string, string>("TTSMessage", xTTSMessage));
                keyValuePairs.Add(new KeyValuePair<string, string>("ExecutablePath", xExecutablePath));
                keyValuePairs.Add(new KeyValuePair<string, string>("ExecutableArguments", xExecutableArguments));

                if (element is null) {
                    XMLHelper.SaveXMLNode(AppViewModel.Instance.XEvents, "Events", "Event", xKeyString, keyValuePairs);
                }
                else {
                    XElement xIsEnabledElement = element.Element("IsEnabled");
                    if (xIsEnabledElement is not null) {
                        xIsEnabledElement.Value = xIsEnabled;
                    }
                    else {
                        element.Add(new XElement("IsEnabled", xIsEnabled));
                    }

                    XElement xRegExElement = element.Element("RegEx");
                    if (xRegExElement is not null) {
                        xRegExElement.Value = xRegEx;
                    }
                    else {
                        element.Add(new XElement("RegEx", xRegEx));
                    }

                    XElement xCategoryElement = element.Element("Category");
                    if (xCategoryElement is not null) {
                        xCategoryElement.Value = xCategory;
                    }
                    else {
                        element.Add(new XElement("Category", xCategory));
                    }

                    XElement xSoundElement = element.Element("Sound");
                    if (xSoundElement is not null) {
                        xSoundElement.Value = xSound;
                    }
                    else {
                        element.Add(new XElement("Sound", xSound));
                    }

                    XElement xVolumeElement = element.Element("Volume");
                    if (xVolumeElement is not null) {
                        xVolumeElement.Value = xVolume;
                    }
                    else {
                        element.Add(new XElement("Volume", xVolume));
                    }

                    XElement xSpeechRateElement = element.Element("SpeechRate");
                    if (xSpeechRateElement is not null) {
                        xSpeechRateElement.Value = xSpeechRate;
                    }
                    else {
                        element.Add(new XElement("SpeechRate", xSpeechRate));
                    }

                    XElement xTTSMessageElement = element.Element("TTSMessage");
                    if (xTTSMessageElement is not null) {
                        xTTSMessageElement.Value = xTTSMessage;
                    }
                    else {
                        element.Add(new XElement("TTSMessage", xTTSMessage));
                    }

                    XElement xExecutablePathElement = element.Element("ExecutablePath");
                    if (xExecutablePathElement is not null) {
                        xExecutablePathElement.Value = xExecutablePath;
                    }
                    else {
                        element.Add(new XElement("ExecutablePath", xExecutablePath));
                    }

                    XElement xExecutableArgumentsElement = element.Element("ExecutableArguments");
                    if (xExecutableArgumentsElement is not null) {
                        xExecutableArgumentsElement.Value = xExecutableArguments;
                    }
                    else {
                        element.Add(new XElement("ExecutableArguments", xExecutableArguments));
                    }
                }
            }

            AppViewModel.Instance.XEvents.Save(Path.Combine(AppViewModel.Instance.SettingsPath, "Events.xml"));
        }
    }
}