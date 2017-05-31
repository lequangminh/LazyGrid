using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LazyGrid
{
	public partial class Form1 : Form
	{
		private DataTable dt = new DataTable(); // Coi như đây là toàn bộ dữ liệu nằm trong table của Database
		private DataTable temp = new DataTable(); // Coi như đây là toàn bộ dữ liệu đang có trên DataGridView

		void createColumn(DataTable table) // tạo column cho 2 cái DataTable
		{
			table.Columns.Add("ID");
			table.Columns.Add("Name");
		}

		public Form1()
		{
			InitializeComponent();

			createColumn(dt);
			createColumn(temp);

			dataGridView1.DataSource = dt; // gán trước để nó có sẵn Column để lúc add thêm rows không báo lỗi
			for (int i = 0; i < 10000; i++)
			{
				dt.Rows.Add(i, "Name " + i);
			}

			addRows(); // thêm 1 ít dữ liệu đầu tiên
		}

		//Ẩn hiện cái ScrollBars
		ScrollBars gridscrollbar;
		private void HideScrollBar()
		{
			gridscrollbar = dataGridView1.ScrollBars;
			dataGridView1.ScrollBars = ScrollBars.None;
		}
		private void ShowScrollBars()
		{
			dataGridView1.ScrollBars = gridscrollbar;
		}

		private int GetDisplayedRowsCount()
		{
			int count = dataGridView1.Rows[dataGridView1.FirstDisplayedScrollingRowIndex].Height;
			count = dataGridView1.Height / count;
			return count;
		}

		private const int limit = 20; // giới hạn 20 rows mỗi lần load thêm
		private int current = 0; // xác định số rows hiện tại
		DateTime lastLoading; // Lưu lại thời gian lần trước load
		int firstVisibleRow; // Rows đầu tiên đang hiển thị
		void addRows()
		{
			HideScrollBar();
			lastLoading = DateTime.Now;
			int total_after = limit + current;
			while (current < total_after)
			{
				if (dt.Rows.Count >= total_after)
				{
					DataRow dr = dt.Rows[current++];

					temp.Rows.Add(dr[0], dr[1]);
				}
				else
					break;
			}
			if (firstVisibleRow > -1)
			{
				ShowScrollBars();
				dataGridView1.FirstDisplayedScrollingRowIndex = firstVisibleRow;
			}
			dataGridView1.DataSource = temp;
		}

		private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.Type == ScrollEventType.SmallIncrement || e.Type == ScrollEventType.LargeIncrement) // Kiểm tra scroll down
			{
				if (e.NewValue >= dataGridView1.Rows.Count - GetDisplayedRowsCount())
				{
					TimeSpan ts = DateTime.Now - lastLoading;
					if (ts.TotalMilliseconds > 100)
					{
						firstVisibleRow = e.NewValue;
						addRows();
					}
					else
					{
						dataGridView1.FirstDisplayedScrollingRowIndex = e.OldValue;
					}
				}
			}
		}
	}
}
