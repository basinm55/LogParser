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
using static Entities.Enums;

namespace LogParserApp
{
    public class TagArrowInfo
    {
        public StateObject stateObj { get; set; }
        public ParserObject refObj { get; set; }
        public bool IsClickable { get; set; }
        public string ToolTipText { get; set; }
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
            int maxDescLength = (int)Utils.GetConfigValue<int>("MaxVisualDescriptionLength");
            maxDescLength = maxDescLength == 0 ? 30 : maxDescLength;

            dataGV.AutoGenerateColumns = false;            
            dataGV.Columns.Clear();            

            var columnsCount = data.Count > 0 ? data.Max(x => x.StateCollection.Count()): 0;

            //The first column is Timeline column!
            columnsCount++;
            var col = new DataGridViewColumn();
            col.Name = "Timeline";
            col.CellTemplate = new DataGridViewTextBoxCell();
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            col.Width = 20;
            col.DividerWidth = 10;
            dataGV.Columns.Add(col);

            for (int i = 1; i < columnsCount; i++)
            {
                col = new DataGridViewColumn();
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
            dataGV.AutoResizeColumns();
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

            CreateTimeLineGridCell(visualStateCollection, row, 0);

            for (int i=1; i < dataGV.ColumnCount; i++)
            {
                var j = i - 1;
                if (j < visualStateCollection.Count)
                {
                    if (visualStateCollection[j] != null)
                    {
                        if (visualStateCollection[j].State == State.ViewArrow)
                        {
                            if (visualStateCollection[j].ReferenceDirection == RefDirection.Backward)
                                CreateArrowImageCell(visualStateCollection[j], row, i, null, obj.PrevInterruptedObj);
                            else if (visualStateCollection[j].ReferenceDirection == RefDirection.Forward)
                                CreateArrowImageCell(visualStateCollection[j], row, i, obj.NextContinuedObj, null);
                        }
                        else
                            CreateGridCell(visualStateCollection[j], row, i, maxDescLength);
                    }

                }               
            }        
            
        }     

        private static void CreateArrowImageCell(StateObject stateObj, DataGridViewRow row, int cellIndex, ParserObject nextContinuedObj, ParserObject prevInterruptedObj)
        {
            var cell = new DataGridViewImageCell();

            if (stateObj == null)
            {
                row.Cells[cellIndex] = cell;
                return;
            }

            if (prevInterruptedObj != null)
            {
                var tag = new TagArrowInfo { refObj = prevInterruptedObj, stateObj = stateObj };
                if (IsArrowClickable(tag))
                {
                    cell.Value = ImageExt.ColorReplace(Properties.Resources.forward_arrow, Color.White, ColorTranslator.FromHtml(prevInterruptedObj.BaseColor));
                    tag.IsClickable = true;
                    tag.ToolTipText = "To PREVIOUS state...";
                    cell.Tag = tag;
                }
                else
                {
                    cell.Value = Properties.Resources.forward_arrow;
                    tag.IsClickable = false;
                    cell.Tag = new TagArrowInfo { refObj = null, stateObj = stateObj };
                }
            }
            else if (nextContinuedObj != null)
            {  
                var tag = new TagArrowInfo { refObj = nextContinuedObj, stateObj = stateObj };
                if (IsArrowClickable(tag))
                {                    
                    cell.Value = ImageExt.ColorReplace(Properties.Resources.forward_arrow, Color.White, ColorTranslator.FromHtml(nextContinuedObj.BaseColor));
                    tag.IsClickable = true;
                    tag.ToolTipText = "To NEXT state...";
                    cell.Tag = tag;
                }
                else
                {
                    cell.Value = Properties.Resources.forward_arrow;
                    tag.IsClickable = false;
                    cell.Tag = new TagArrowInfo { refObj = null, stateObj = stateObj };
                }

            }            
            else
            {
                cell.Value = Properties.Resources.forward_arrow;
                cell.Tag = new TagArrowInfo { refObj = null, stateObj = stateObj };
            }

            row.Cells[cellIndex] = cell;
        }

        private static bool IsArrowClickable(TagArrowInfo tag)
        {

            if (tag != null)
            {
                if (tag.refObj is ParserObject &&
                    (tag.refObj.NextContinuedObj != null || tag.refObj.PrevInterruptedObj != null))
                {
                    int idx = tag.stateObj.Parent.StateCollection.IndexOf(tag.stateObj);
                    if (idx > 0 && idx < tag.stateObj.Parent.StateCollection.Count)
                    {
                        var nextState = idx < tag.stateObj.Parent.StateCollection.Count - 1 ?
                            tag.stateObj.Parent.StateCollection[idx + 1] : null;
                        var prevState = tag.stateObj.Parent.StateCollection[idx - 1];
                        if (nextState == null || nextState.ObjectClass == ObjectClass.Blank || prevState.ObjectClass == ObjectClass.Blank)
                            return true;
                    }
                }
            }
            return false;
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
                BackColor = stateObj.State == State.Missing ? Color.White : ColorTranslator.FromHtml(stateObj.Color),
                ForeColor = stateObj.State == State.Missing ? Color.Black : Color.Black,
                Font = stateObj.State == State.Missing ?
                    new Font(Control.DefaultFont, FontStyle.Bold) :
                    new Font(Control.DefaultFont, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5, 5, 5, 5),
                SelectionBackColor = Color.DarkOrange
            };


            if (stateObj.State != State.Blank)
            {
                var stateDescription = CreateVisualDescription(stateObj, maxDescLength);
                stateObj.Description = stateDescription.ToString();
                cell.Value = stateObj;
            }            
            else
                cell.Value = null;

            row.Cells[cellIndex] = cell;            
        }

        private static void CreateTimeLineGridCell(List<StateObject> stateCollection, DataGridViewRow row, int cellIndex)
        {
            var cell = new DataGridViewTextBoxCell();

            var stateObj = stateCollection.FirstOrDefault(x => x.Time > DateTime.MinValue);
            if (stateObj == null)
            {
                row.Cells[cellIndex] = cell;
                return;
            }

            cell.Style = new DataGridViewCellStyle
            {
                BackColor = Color.LightGoldenrodYellow,
                ForeColor = Color.Black,                
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                //Font = new Font(FontFamily.GenericSerif.Name, 7F)
            };                                 
            var strTime = stateObj.Time.ToString("mm:ss.FFF");
            cell.Tag = stateObj.Time;
            cell.Value = strTime.Replace(".", Environment.NewLine + ".");
            row.Cells[cellIndex] = cell;
        }

        private static StringBuilder CreateVisualDescription(StateObject stateObj, int maxDescLength)
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendLine(stateObj.State.ToString() 
                + (stateObj.State == State.Missing ? "..." : string.Empty));
           
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
