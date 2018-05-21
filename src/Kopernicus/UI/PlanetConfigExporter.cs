﻿/**
 * Kopernicus Planetary System Modifier
 * ------------------------------------------------------------- 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright 2011-2017 Squad. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Kopernicus.Configuration;
using KSP.Localization;
using Object = System.Object;

namespace Kopernicus
{
    namespace UI
    {
        /// <summary>
        /// Converts a celestial body into a Kopernicus config
        /// </summary>
        public static class PlanetConfigExporter
        {
            public static ConfigNode CreateConfig(CelestialBody celestialBody)
            {
                return CreateConfig(new Body(celestialBody));
            }
            
            public static ConfigNode CreateConfig(Body body)
            {
                // Create the ConfigNode
                ConfigNode bodyNode = new ConfigNode("Body");

                // Export the planet to the config
                WriteToConfig(body, ref bodyNode);

                return bodyNode;
            }

            private static void WriteToConfig(Object value, ref ConfigNode node)
            {
                // If the value can export to config node directly
                if (value is IConfigNodeWritable)
                {
                    ConfigNode values = ((IConfigNodeWritable) value).ValueToNode();
                    if (values != null)
                    {
                        node.AddData(values);
                    }

                    return;
                }

                // Get all ParserTargets from the object
                Dictionary<ParserTarget, MemberInfo> parserTargets = Tools.GetParserTargets(value.GetType());

                // Export all found targets
                foreach (KeyValuePair<ParserTarget, MemberInfo> keyValuePair in parserTargets)
                {
                    ParserTarget parserTarget = keyValuePair.Key;
                    MemberInfo memberInfo = keyValuePair.Value;

                    // Is this a collection or a single value?
                    if (Tools.IsCollection(parserTarget))
                    {
                        ProcessCollection(parserTarget, memberInfo, value, ref node);
                    }
                    else
                    {
                        ProcessSingleValue(parserTarget, memberInfo, value, ref node);
                    }
                }
            }

            /// <summary>
            /// Adds a single value ParserTarget to the ConfigNode tree
            /// </summary>
            private static void ProcessSingleValue(ParserTarget parserTarget, MemberInfo memberInfo, Object reference,
                ref ConfigNode node)
            {
                // Get the value of the MemberInfo
                Object value = Tools.GetValue(memberInfo, reference);
                if (value == null)
                {
                    return;
                }

                // Is this a value or a node?
                ConfigType configType = Tools.GetConfigType(value.GetType());
                if (configType == ConfigType.Value)
                {
                    SetValue(parserTarget, memberInfo, reference, ref node);
                }
                else
                {
                    // Create the new node
                    String name = parserTarget.FieldName;
                    if (parserTarget.NameSignificance == NameSignificance.Type)
                    {
                        name += ":" + value.GetType().Name;
                    }

                    ConfigNode valueNode = node.AddNode(name);
                    WriteToConfig(value, ref valueNode);
                }
            }

            /// <summary>
            /// Adds a multi-value ParserTarget to the ConfigNode tree
            /// </summary>
            private static void ProcessCollection(ParserTarget parserTarget, MemberInfo memberInfo, Object reference,
                ref ConfigNode node)
            {
                // Get the type of the collection
                Type memberType = Tools.MemberType(memberInfo);

                // Is the collection a dictionary?
                if (typeof(IDictionary).IsAssignableFrom(memberType))
                {
                    IDictionary dictionary = Tools.GetValue(memberInfo, reference) as IDictionary;

                    // Is the dictionary null?
                    if (dictionary == null)
                    {
                        return;
                    }

                    // Create the new ConfigNode
                    ConfigNode targetNode = null;

                    // Iterate over the elements of the dictionary
                    foreach (DictionaryEntry value in dictionary)
                    {
                        // Null-Check
                        if (value.Key == null || value.Value == null)
                        {
                            continue;
                        }

                        // Create the node if neccessary
                        if (targetNode == null)
                        {
                            targetNode = node;
                            if (parserTarget.FieldName != "self")
                            {
                                targetNode = node.AddNode(parserTarget.FieldName);
                            }
                        }

                        // The first generic type has to be ConfigType.Value, figure out the type of the second one
                        ConfigType type = Tools.GetConfigType(value.Value.GetType());

                        // If it is a node, add it to the node
                        if (type == ConfigType.Node)
                        {
                            ConfigNode valueNode = targetNode.AddNode(Tools.FormatParsable(value.Key));
                            WriteToConfig(value.Value, ref valueNode);
                        }
                        else
                        {
                            targetNode.AddValue(Tools.FormatParsable(value.Key), Tools.FormatParsable(value.Value));
                        }
                    }
                }
                else if (typeof(IList).IsAssignableFrom(memberType))
                {
                    IList list = Tools.GetValue(memberInfo, reference) as IList;

                    // Is the dictionary null?
                    if (list == null)
                    {
                        return;
                    }

                    // Create the new ConfigNode
                    ConfigNode targetNode = null;

                    // Iterate over the elements of the list
                    foreach (Object value in list)
                    {
                        // Null-Check
                        if (value == null)
                        {
                            continue;
                        }

                        // Create the node if neccessary
                        if (targetNode == null)
                        {
                            targetNode = node;
                            if (parserTarget.FieldName != "self")
                            {
                                targetNode = node.AddNode(parserTarget.FieldName);
                            }
                        }

                        // Figure out the config type of type
                        ConfigType type = Tools.GetConfigType(value.GetType());

                        // If it is a node, add it to the node
                        if (type == ConfigType.Node)
                        {
                            String name = "Value";
                            if (parserTarget.NameSignificance == NameSignificance.Key)
                            {
                                name = parserTarget.Key;
                            }

                            if (parserTarget.NameSignificance == NameSignificance.Type)
                            {
                                name = value.GetType().Name;
                            }

                            ConfigNode valueNode = targetNode.AddNode(name);
                            WriteToConfig(value, ref valueNode);
                        }
                        else
                        {
                            String name = "value";
                            if (parserTarget.NameSignificance == NameSignificance.Key)
                            {
                                name = parserTarget.Key;
                            }

                            if (parserTarget.NameSignificance == NameSignificance.Type)
                            {
                                name = value.GetType().Name;
                            }

                            targetNode.AddValue(name, Tools.FormatParsable(value));
                        }
                    }
                }
            }

            /// <summary>
            /// Formats and adds a ParserTarget to a ConfigNode
            /// </summary>
            private static void SetValue(ParserTarget parserTarget, MemberInfo memberInfo, Object reference,
                ref ConfigNode node)
            {
                // Get the value behind the MemberInfo
                Object value = Tools.GetValue(memberInfo, reference);
                if (value == null)
                {
                    return;
                }

                // Format the value
                String formattedValue = Tools.FormatParsable(value);
                if (formattedValue == null)
                {
                    return;
                }

                formattedValue = Localizer.Format(formattedValue);

                // Get a description
                String description = Tools.GetDescription(memberInfo);

                // Add it to the config
                if (String.IsNullOrEmpty(description))
                {
                    node.AddValue(parserTarget.FieldName, formattedValue);
                }
                else
                {
                    node.AddValue(parserTarget.FieldName, formattedValue, description);
                }
            }
        }
    }
}