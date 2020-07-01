using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
            if (data.Count == 0) return;

            dataGV.AutoGenerateColumns = false;            
            dataGV.Columns.Clear();
            

            var columnsCount = data.Max(x => x.VisualObjectCollection.Count());           

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

           
            foreach (var row in data)
            {                  
                CreateGridRow(row, dataGV, deviceFilter);
            }

            SetGridParameters(dataGV);
     
            dataGV.ClearSelection();       
        }

        private static void CreateGridRow(ParserObject obj, DataGridView dataGV, string device)
        {
            if (obj == null) return;
            List<ParserObject> parserRowCollection;
            if (device != null)
                parserRowCollection = obj.VisualObjectCollection.Where(o => o==null ||(string)o.GetDynPropertyValue("Parent") == device).ToList();
            else
                parserRowCollection = obj.VisualObjectCollection;

            if (parserRowCollection.Count == 0) return;

            var rowIndex = dataGV.Rows.Add();
            var row = dataGV.Rows[rowIndex];
            
            for (int i=0; i<dataGV.ColumnCount; i++)
            {  
  
                if (i < parserRowCollection.Count)
                    CreateGridCell(parserRowCollection[i], row, i);
                else
                    CreateGridCell(row, i);
            }        
            
        }

        private static void CreateGridCell(ParserObject visualObj, DataGridViewRow row, int cellIndex)
        {                     
            var cell = new DataGridViewTextBoxCell();
            if (visualObj == null)
            {
                row.Cells[cellIndex] = cell;
                return;
            }

            //currentColor = currentColor == Color.Transparent ?                
            //    ColorTranslator.FromHtml("#87cefa") ://Color.LightSkyBlue :
            //    Utils.DarkerColor(currentColor, 10f);


            cell.Style = new DataGridViewCellStyle
            {
                BackColor = visualObj.ObjectColor,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5, 5, 5, 5),
                SelectionBackColor = Color.DarkOrange
            };

            var visualDescription = new StringBuilder();
            visualDescription.AppendLine(visualObj.ObjectType.ToString());
            visualDescription.AppendLine((string)visualObj.GetDynPropertyValue("this"));
            //visualDescription.AppendLine((string)visualObj.GetDynPropertyValue("FilterKey"));      
            visualDescription.AppendLine(visualObj.ObjectState.ToString());
            visualObj.VisualDescription = visualDescription.ToString();
            if (visualObj == null || string.IsNullOrWhiteSpace(visualObj.VisualDescription))
                visualObj.VisualDescription = null;
            cell.Value = visualObj;


            row.Cells[cellIndex] = cell; 
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
