using Habanero.DB;
using System.Data;
using System.Data.SqlClient;

namespace NoteTakingApp
{
    public partial class NoteTaker : Form
    {
        DataTable notes = new DataTable();
        bool editing = false;
        private static int nextId = 1;

        public NoteTaker()
        {
            InitializeComponent();
        }

        private void pullData()
        {
            string connString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = NoteTakingDB; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string select = "select * from Notes";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(select, conn);
            conn.Open();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(notes);
            da.Dispose();
        }

        private void NoteTaker_Load(object sender, EventArgs e)
        {
            
            notes.Columns.Add("Id");
            notes.Columns.Add("Title");
            notes.Columns.Add("Note");

            previousNotes.DataSource = notes;

            pullData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (previousNotes.SelectedRows.Count == 0)
            {
                return;
            }

            var itemToDelete = previousNotes.SelectedRows[0].Index;
            string connString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = NoteTakingDB; Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            using (var connection = new SqlConnection(connString))
            {
                connection.Open();

                using (var command = new SqlCommand("DELETE FROM Notes WHERE Id=" + previousNotes.SelectedRows[0].Index))
                {
                    command.Parameters.AddWithValue(previousNotes.SelectedRows[0].Index.ToString(), itemToDelete);
                    command.Connection = connection;
                    command.ExecuteNonQuery();

                    string query = "DELETE FROM Notes WHERE ID = " + previousNotes.SelectedRows[0].Index;
                    DatabaseConnection.sqlCommandQueryReader(query);
                    // delete item from database
                    MessageBox.Show("Information deleted");
                    
                    // delete item from datasource and update DGV
                    notes.Rows[previousNotes.CurrentCell.RowIndex].Delete();
                }
            }



            ////int i = Convert.ToInt32(notes.Rows[previousNotes.CurrentCell.ColumnIndex]);
            //int position = Convert.ToInt32(notes.Rows[previousNotes.CurrentCell.RowIndex]);

            //Object cellValue = notes.Rows[position];
            
            //string delete = "delete from Notes where Id ='" + position;

            //try
            //{
            //    notes.Rows[previousNotes.CurrentCell.RowIndex].Delete();
            //}
            //catch (Exception)
            //{
            //    Console.WriteLine("Not a valid note");
            //}

            //DatabaseConnection.sqlCommandQueryReader(delete);
            //    MessageBox.Show("Title has been deleted!");
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (editing)
            {
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Id"] = nextId++;
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Title"] = TitleBox.Text;
                notes.Rows[previousNotes.CurrentCell.RowIndex]["Note"] = NoteBox.Text;
            }
            else
            {
                notes.Rows.Add(nextId++, TitleBox.Text, NoteBox.Text);
                string insertt = "insert into Notes(Id,Title,Note) values('" + nextId++ + "','" + TitleBox.Text + "','" + NoteBox.Text + "');";
                DatabaseConnection.sqlCommandQueryReader(insertt);
                MessageBox.Show("Information inserted!");
            }
            nextId++;
            TitleBox.Text = "";
            NoteBox.Text = "";
            editing = false;
        }

        private void NewNoteButton_Click(object sender, EventArgs e)
        {
            
            string Title = TitleBox.Text;
            string Note = NoteBox.Text;

            string insertt = "insert into Notes(Id,Title,Note) values('" + nextId++ + "','" +Title + "','" + Note + "');";

            DatabaseConnection.sqlCommandQueryReader(insertt);
            MessageBox.Show("Information inserted!");
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {


            TitleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            NoteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            editing = true;
        }

        private void previousNotes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            TitleBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[0].ToString();
            NoteBox.Text = notes.Rows[previousNotes.CurrentCell.RowIndex].ItemArray[1].ToString();
            editing = true;
        }
    }
}