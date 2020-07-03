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
        public static void CreateGridView(List<ParserObject> data, DataGridView dataGV, string deviceFilter)
        {
            //if (data.Count == 0) return;
            int maxDescLength = (int)Utils.GetConfigValue<int>("MaxVisualDescriptionLength");
            maxDescLength = maxDescLength == 0 ? 30 : maxDescLength;

            dataGV.AutoGenerateColumns = false;            
            dataGV.Columns.Clear();

            foreach (var o in data)
            {
                var voCollection = o.VisualObjectCollection;
                voCollection.RemoveAll((x) => x == null);
            }
            
            var columnsCount = data.Count > 0 ? data.Max(x => x.VisualObjectCollection.Count()*2 - 1): 0;           

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

        private static void CreateGridRow(ParserObject obj, DataGridView dataGV, string device, int maxDescLength)
        {
            if (obj == null) return;
            List<ParserObject> parserRowCollection;
            if (device != null)
                parserRowCollection = obj.VisualObjectCollection.Where(o => o == null || (string)o.GetDynPropertyValue("Parent") == device).ToList();
            else
                parserRowCollection = obj.VisualObjectCollection;

            if (parserRowCollection.Count == 0) return;

            //Insert places for ForwardArrow images
            var parserRowCollectionCount = parserRowCollection.Count * 2 - 1;
            for (int i=0; i<parserRowCollectionCount; i++)
            {                  
                parserRowCollection.Insert(i+1, null);
                i++;
            }

            var rowIndex = dataGV.Rows.Add();
            var row = dataGV.Rows[rowIndex];
            
            for (int i=0; i<dataGV.ColumnCount; i++)
            {

                if (i < parserRowCollectionCount)
                {
                    if (i % 2 == 0 || parserRowCollection[i] != null)
                        CreateGridCell(parserRowCollection[i], row, i, maxDescLength);                   
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

        private static void CreateGridCell(ParserObject visualObj, DataGridViewRow row, int cellIndex, int maxDescLength)
        {                     
            var cell = new DataGridViewTextBoxCell();
            var cellForwardImg = new DataGridViewImageCell();
            cellForwardImg.Value = Properties.Resources.forvard_arrow;       

            if (visualObj == null)
            {
                row.Cells[cellIndex] = cell;
                return;
            }
           

            cell.Style = new DataGridViewCellStyle
            {
                BackColor = visualObj.ObjectState == Enums.ObjectState.Dropped ? Color.Black : visualObj.ObjectColor,
                ForeColor = visualObj.ObjectState == Enums.ObjectState.Dropped ? Color.White : Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5, 5, 5, 5),
                SelectionBackColor = Color.DarkOrange
            };


            var visualDescription = CreateVisualDescription(visualObj, maxDescLength);                           
            visualObj.VisualDescription = visualDescription.ToString();
            if (visualObj == null || string.IsNullOrWhiteSpace(visualObj.VisualDescription))
                visualObj.VisualDescription = null;
            cell.Value = visualObj;



            row.Cells[cellIndex] = cell;            
        }

        private static StringBuilder CreateVisualDescription(ParserObject visualObj, int maxDescLength)
        {
            var visualDescription = new StringBuilder();
            visualDescription.AppendLine(visualObj.ObjectState.ToString());
            if (visualObj.ObjectDescription != null)
            {
                foreach (var desc in visualObj.ObjectDescription)
                {
                    var description = desc.Value;
                    if (description.Length > maxDescLength)
                        description = StringExt.Wrap(desc.Value, maxDescLength);

                    if ((desc.Key + ": " + description).Length > maxDescLength)
                        visualDescription.AppendLine(description);
                    else if (desc.Key.ToLower() == "request")
                        visualDescription.AppendLine(description);
                    else
                        visualDescription.AppendLine(desc.Key + ": " + description);

                }
            }
            return visualDescription;
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
