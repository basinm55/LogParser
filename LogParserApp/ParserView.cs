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
        public static void CreateGridView(List<ParserObject> data, DataGridView dataGV)
        {
            //var data1 = new List<MyStruct>() { new MyStruct("a", "b"), new MyStruct("c", "d") };     

            //var source = new BindingSource();
            //source.DataSource = data;
            dataGV.AutoGenerateColumns = false;
            //dataGV.DataSource = source;

            dataGV.Columns.Clear();

            var columnsCount = data.Max(x => x.VisualObjectCollection.Distinct().Count());
            //dataGV.ColumnCount = columnsCount;

            for (int i = 0; i < columnsCount; i++)
            {
                var col = new DataGridViewColumn();
                col.CellTemplate = new DataGridViewTextBoxCell();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                //col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;               
                col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                col.Width = 100;
                col.DividerWidth = 10;                
                dataGV.Columns.Add(col);                      
            }


            for (int i = 0; i < columnsCount; i++)
            {
                CreateGridRow(data[i], dataGV.Columns[i], dataGV);
            }


            foreach (var c in dataGV.Columns)
            {
                ((DataGridViewColumn)c).Width = 100;
                ((DataGridViewColumn)c).DividerWidth = 10;
            }

            foreach (var r in dataGV.Rows)
            {
                ((DataGridViewRow)r).Height = 100;
                ((DataGridViewRow)r).DividerHeight = 10;
            }

            dataGV.Refresh();
            dataGV.ClearSelection();       
        }

        private static void CreateGridRow(ParserObject obj, DataGridViewColumn column, DataGridView dataGV)
        {
            if (obj == null) return;
            var parserRowCollection = obj.VisualObjectCollection;
            if (parserRowCollection.Count == 0) return;

            var rowIndex = dataGV.Rows.Add();
            var row = dataGV.Rows[rowIndex];

            Color currentColor = Color.Transparent;
            for (int i=0; i<dataGV.ColumnCount; i++)
            {  
  
                if (i < parserRowCollection.Count)
                    currentColor = CreateGridCell(parserRowCollection[i], row, i, currentColor);
                else
                    CreateGridCell(row, i);
            }        
            
        }

        private static Color CreateGridCell(ParserObject visualObj, DataGridViewRow row, int cellIndex, Color currentColor)
        {
            //Color col = ColorConverter.ConvertFromString("#FFDFD991") as Color;
            //string colorcode = "#FFDFD991";                       

            var cell = new DataGridViewTextBoxCell();

            currentColor = currentColor == Color.Transparent ?
                Color.FromName((string)visualObj.GetDynPropertyValue("BaseColor")) :
                Utils.DarkerColor(currentColor, 20f);           

            cell.Style = new DataGridViewCellStyle
            {
                BackColor = currentColor,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5, 5, 5, 5)

            };

            var visualDescription = new StringBuilder();
            visualDescription.AppendLine(visualObj.ObjectClass+":");
            visualDescription.AppendLine((string)visualObj.GetDynPropertyValue("this"));
            visualDescription.AppendLine((string)visualObj.GetDynPropertyValue("FilterKey"));      
            visualDescription.AppendLine((string)visualObj.GetDynPropertyValue("State"));
            cell.Value = visualDescription.ToString();


            row.Cells[cellIndex] = cell;

            return cell.Style.BackColor;
        }

        private static void CreateGridCell(DataGridViewRow row, int cellIndex)
        {
            //Create an emoty cell
            var cell = new DataGridViewTextBoxCell();
            cell.Value = string.Empty;
            row.Cells[cellIndex] = cell;           
        }
    }
}
