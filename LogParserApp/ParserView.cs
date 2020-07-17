using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entities;
using Helpers;

namespace LogParserApp
{
    public class TagArrowInfo
    {
        public StateObject stateObj { get; set; }
        public ParserObject refObj { get; set; }
    }

    public partial class ParserView
    {
        //string[] _displayProperties;

        public static bool AllowedForDisplayProperties(string propName, ref string[] displayInInfoboxProps)
        {
            if (displayInInfoboxProps != null && displayInInfoboxProps.Length > 0)
                return displayInInfoboxProps.Contains(propName);

            var props = ConfigurationManager.AppSettings["DisplayInInfobox"];
            if (props != null)
                displayInInfoboxProps = props.Split(',');
            else
            {
                displayInInfoboxProps = new string[]
                {
                    "this",
                    "Parent",
                    "State",
                    "Line",
                    "LineNum",
                    "Port",
                    "ID"
                };
            }          

            return displayInInfoboxProps.Contains(propName);
        }

        public static void CreateGridView(List<ParserObject> data, DataGridView dataGV, string deviceFilter)
        {          

            //if (data.Count == 0) return;
            int maxDescLength = (int)Utils.GetConfigValue<int>("MaxVisualDescriptionLength");
            maxDescLength = maxDescLength == 0 ? 30 : maxDescLength;

            dataGV.AutoGenerateColumns = false;            
            dataGV.Columns.Clear();

            //foreach (var o in data)
            //{
            //    if (o == null) continue;
            //    var stateCollection = o.StateCollection;
            //    stateCollection.RemoveAll((x) => x == null);
            //}
            
            var columnsCount = data.Count > 0 ? data.Max(x => x.StateCollection.Count()): 0;           

            for (int i = 0; i < columnsCount; i++)
            {
                var col = new DataGridViewColumn();
                col.CellTemplate = new DataGridViewTextBoxCell();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;                               
                col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                col.Width = 100;
                col.DividerWidth = 10;                
                dataGV.Columns.Add(col);                      
            }

            if (data.Count > 0)
            {
                foreach (var row in data)
                {
                    CreateGridRow(row, dataGV, deviceFilter, maxDescLength);
                }

                SetGridParameters(dataGV);
            }
            dataGV.ClearSelection();       
        }

        private static void CreateGridRow(ParserObject obj, DataGridView dataGV, string thisOfDevice, int maxDescLength)
        {
            if (obj == null) return;
            List<StateObject> visualStateCollection;
            if (!string.IsNullOrWhiteSpace(thisOfDevice))
                visualStateCollection = obj.StateCollection.Where(o => o == null || o.Parent.GetParent() == thisOfDevice).ToList();
            else
                visualStateCollection = obj.StateCollection;

            if (visualStateCollection.Count == 0) return;           

            var rowIndex = dataGV.Rows.Add();
            var row = dataGV.Rows[rowIndex];
            
            for (int i=0; i < dataGV.ColumnCount; i++)
            {

                if (i < visualStateCollection.Count)
                {
                    if (visualStateCollection[i] != null)
                    {
                        if (visualStateCollection[i].State == Enums.State.ViewArrow)
                            CreateForwardImadeCell(visualStateCollection[i], row, i, obj.NextContinuedObj, obj.PrevInterruptedObj);
                        else
                            CreateGridCell(visualStateCollection[i], row, i, maxDescLength);                                              
                    }

                }               
            }        
            
        }

        private static void CreateForwardImadeCell(StateObject stateObj, DataGridViewRow row, int cellIndex, ParserObject nextContinuedObj, ParserObject prevInterruptedObj)
        {            
            var cell = new DataGridViewImageCell();

            if (stateObj == null)
            {
                row.Cells[cellIndex] = cell;
                return;
            }

            if (nextContinuedObj != null)
            {
                cell.Value = ImageExt.ColorReplace(Properties.Resources.forward_arrow, Color.White, nextContinuedObj.BaseColor);
                cell.Tag = new TagArrowInfo{ refObj = nextContinuedObj, stateObj = stateObj };
            }
            else if (prevInterruptedObj != null)
            {
                var selfIdx = stateObj.Parent.StateCollection.IndexOf(stateObj);
                if (selfIdx > 0 && stateObj.Parent.StateCollection[selfIdx - 1].ObjectClass == Enums.ObjectClass.Empty)
                    cell.Value = ImageExt.ColorReplace(Properties.Resources.backward_arrow, Color.White, prevInterruptedObj.BaseColor);
                else
                    cell.Value = ImageExt.ColorReplace(Properties.Resources.forward_arrow, Color.White, prevInterruptedObj.BaseColor);
                cell.Tag = new TagArrowInfo { refObj = prevInterruptedObj, stateObj = stateObj };
            }
            else
            {
                cell.Value = Properties.Resources.forward_arrow;
                cell.Tag = new TagArrowInfo { refObj = null, stateObj = stateObj }; ;
            }

            row.Cells[cellIndex] = cell;
        }

        private static void CreateGridCell(StateObject stateObj, DataGridViewRow row, int cellIndex, int maxDescLength)
        {                     
            var cell = new DataGridViewTextBoxCell();            
    
            if (stateObj == null)
            {
                row.Cells[cellIndex] = cell;
                return;
            }
           

            cell.Style = new DataGridViewCellStyle
            {
                BackColor = stateObj.Color,   
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5, 5, 5, 5),
                SelectionBackColor = Color.DarkOrange
            };


            if (stateObj.State != Enums.State.Empty)
            {
                var stateDescription = CreateVisualDescription(stateObj, maxDescLength);
                stateObj.Description = stateDescription.ToString();
                cell.Value = stateObj;
            }            
            else
                cell.Value = null;

            row.Cells[cellIndex] = cell;            
        }

        private static StringBuilder CreateVisualDescription(StateObject stateObj, int maxDescLength)
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendLine(stateObj.State.ToString());
           
            foreach (var desc in stateObj.VisualDescription)
            {
                var description = desc.Value;
                if (description.Length > maxDescLength)
                    description = StringExt.Wrap(desc.Value, maxDescLength);

                if ((desc.Key + ": " + description).Length > maxDescLength)
                    stateDescription.AppendLine(description);
                else if (desc.Key.ToLower() == "request")
                    stateDescription.AppendLine(description);
                else
                    stateDescription.AppendLine(desc.Key + ": " + description);

                }

            return stateDescription;
        }

        private static void CreateGridCell(DataGridViewRow row, int cellIndex)
        {
            //Create an empty cell
            var cell = new DataGridViewTextBoxCell();
            cell.Value = string.Empty;
            row.Cells[cellIndex] = cell;           
        }

        private static void SetGridParameters(DataGridView dataGV)
        {
            foreach (var c in dataGV.Columns)
            {
                ((DataGridViewColumn)c).Width = 100;
                ((DataGridViewColumn)c).DividerWidth = 10;
            }

            foreach (var r in dataGV.Rows)
            {
                ((DataGridViewRow)r).Height = 100;
                ((DataGridViewRow)r).DividerHeight = 20;
            }

        }        
    }
}
