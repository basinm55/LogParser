using System;
using System.Collections.Generic;
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
    public partial class ParserView
    {
        public static bool AllowedForDisplayProperties(string propName)
        {
           string[] allowed =
           {                   
                "this",
                "Parent",                
                "State",
                "Line",
                "LineNum",
                "Port", 
            };

            return allowed.Contains(propName);
        }

        public static void CreateGridView(List<ParserObject> data, DataGridView dataGV, string deviceFilter)
        {
            //if (data.Count == 0) return;
            int maxDescLength = (int)Utils.GetConfigValue<int>("MaxVisualDescriptionLength");
            maxDescLength = maxDescLength == 0 ? 30 : maxDescLength;

            dataGV.AutoGenerateColumns = false;            
            dataGV.Columns.Clear();

            foreach (var o in data)
            {
                if (o == null) continue;
                var stateCollection = o.StateCollection;
                stateCollection.RemoveAll((x) => x == null);
            }
            
            var columnsCount = data.Count > 0 ? data.Where(x => x != null).Max(x => x.StateCollection.Count()*2 - 1): 0;           

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

            //Insert places for ForwardArrow images
            var visualStateCollectionCount = visualStateCollection.Count * 2 - 1;
            for (int i=0; i< visualStateCollectionCount; i++)
            {
                visualStateCollection.Insert(i+1, null);
                i++;
            }

            var rowIndex = dataGV.Rows.Add();
            var row = dataGV.Rows[rowIndex];
            
            for (int i=0; i<dataGV.ColumnCount; i++)
            {

                if (i < visualStateCollectionCount)
                {
                    if (i % 2 == 0 || visualStateCollection[i] != null)
                        CreateGridCell(visualStateCollection[i], row, i, maxDescLength);                   
                    else
                        CreateForwardImadeCell(row, i);

                }
                else
                    CreateGridCell(row, i);
            }        
            
        }

        private static void CreateForwardImadeCell(DataGridViewRow row, int cellIndex)
        {
            var cell = new DataGridViewImageCell();
            cell.Value = Properties.Resources.forvard_arrow;
            row.Cells[cellIndex] = cell;
        }

        private static void CreateGridCell(StateObject stateObj, DataGridViewRow row, int cellIndex, int maxDescLength)
        {                     
            var cell = new DataGridViewTextBoxCell();
            var cellForwardImg = new DataGridViewImageCell();
    
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


            var stateDescription = CreateVisualDescription(stateObj, maxDescLength);                           
            stateObj.Description = stateDescription.ToString();
            //if (string.IsNullOrWhiteSpace(stateObj.Description))
            //    stateObj.Description = null;
            cell.Value = stateObj;
            //cell.Value = stateObj.Description;

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
