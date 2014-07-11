using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Deployment.Application;
using System.Reflection;
using System.Threading;

namespace IVO_Sync_2
{
    public partial class Sync : Form
    {
        private OleDbCommand cmdAccess;
        private SqlCommand cmdPrimavera;
        private SqlCommand cmdPrimaveraWrite;
        private SqlCommand cmdConjunto;
        private OleDbConnection cnnAccess;
        private SqlConnection cnnPrimavera1;
        private SqlConnection cnnPrimavera2;
        private SqlConnection cnnConjunto;
        private OleDbDataReader drAccess;
        private SqlDataReader drPrimavera;
        private SqlDataReader drConjunto;
        public string strSql;
        public int numLinha;
        public int conjunto;

        public Sync()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = @"Server=SERVER-FS01\PRIMAVERA;Database=PRIEIC;User Id=sa;Password=***;";
            cnnPrimavera1 = new SqlConnection(connectionString);
            cnnPrimavera2 = new SqlConnection(connectionString);
            cnnConjunto = new SqlConnection(connectionString);
            string str2 = @"Provider=Microsoft.JET.OLEDB.4.0;data source=S:\Embalagem\EmbalIVO.mdb";
            cnnAccess = new OleDbConnection(str2);
            string str3 = "";
            string name = Assembly.GetExecutingAssembly().GetName().Name;
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                str3 = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            Text = name + " - " + str3;

            lblLinhas.Text = "";
            lblCabecs.Text = "";
        }

        private void btnSync_Click(object sender, EventArgs e)
        {
            btnSync.Enabled = false;
            toolTip1.SetToolTip(btnSync, "A sincronizar...");
            DoBackGroundWork();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void CompareCabecDoc()
        {
            SetControlPropertyValue(lblStatus, "Text", "A inicar sincronização de encomendas...");
            SetControlPropertyValue(lblCabecs, "Text", "");
            SetControlPropertyValue(lblLinhas, "Text", "");
            Thread.Sleep(1000);
            //strSql = "SELECT COUNT(*) FROM CabecDoc WHERE CabecDoc.Serie='2011' and CabecDoc.NumDoc > '312' and CabecDoc.TipoDoc='ECL' AND NOT EXISTS (SELECT * FROM _IVO_RelCabec WHERE _IVO_RelCabec.idCabecDoc = CabecDoc.Id)";
            strSql = "SELECT COUNT(*) FROM CabecDoc WHERE CabecDoc.Serie>'2011' and CabecDoc.TipoDoc='ECL' AND NOT EXISTS (SELECT * FROM _IVO_RelCabec WHERE _IVO_RelCabec.idCabecDoc = CabecDoc.Id)";
            cmdPrimavera = new SqlCommand(strSql, cnnPrimavera1);
            if (cnnPrimavera1.State == ConnectionState.Closed)
            {
                cnnPrimavera1.Open();
            }
            int num = (int)cmdPrimavera.ExecuteScalar();
            int num2 = 1;
            SetControlPropertyValue(lblCabecs, "Text", "A sincronizar " + num.ToString() + " encomendas.");
            cnnPrimavera1.Close();
            strSql = "SELECT CabecDoc.Id, CabecDoc.NumDoc, CabecDoc.Serie, CabecDoc.DataVencimento, CabecDoc.Entidade, CabecDoc.Referencia FROM CabecDoc WHERE CabecDoc.Serie>'2011' and CabecDoc.TipoDoc='ECL' AND NOT EXISTS (SELECT * FROM _IVO_RelCabec WHERE _IVO_RelCabec.idCabecDoc = CabecDoc.Id)";
            if (cnnPrimavera1.State == ConnectionState.Closed)
            {
                cnnPrimavera1.Open();
            }
            cmdPrimavera = new SqlCommand(strSql, cnnPrimavera1);
            drPrimavera = cmdPrimavera.ExecuteReader();
            if (drPrimavera.HasRows)
            {
                while (drPrimavera.Read())
                {
                    string str = drPrimavera[1].ToString();
                    string str2 = drPrimavera[2].ToString();
                    if (CountChars(str) == 1)
                    {
                        str = str2 + "000" + str;
                    }
                    if (CountChars(str) == 2)
                    {
                        str = str2 + "00" + str;
                    }
                    if (CountChars(str) == 3)
                    {
                        str = str2 + "0" + str;
                    }
                    if (CountChars(str) == 4)
                    {
                        str = str2 + str;
                    }
                    string dataSaida = drPrimavera[3].ToString();
                    string idCabecDoc = drPrimavera[0].ToString();
                    string cliente = drPrimavera[4].ToString();
                    string numDocCliente = drPrimavera[5].ToString();
                    WriteAccessCabec(cliente, str, numDocCliente, dataSaida, idCabecDoc);
                    worker.ReportProgress((int)((((double)num2) / ((double)num)) * 100.0));
                    num2++;
                }
            }
            drPrimavera.Close();
            cnnPrimavera1.Close();
            SetControlPropertyValue(lblCabecs, "Text", num.ToString() + " encomendas sincronizadas.");
            Thread.Sleep(1000);
            worker.ReportProgress(0);
            CompareLinhasDoc();
        }

        private void CompareLinhasDoc()
        {
            SetControlPropertyValue(lblStatus, "Text", "A inicar sincronização de linhas...");
            Thread.Sleep(1000);
            strSql = "SELECT COUNT(*) FROM (((LinhasDoc left join CabecDoc on LinhasDoc.IdCabecDoc=CabecDoc.Id) left join _IVO_RelCabec on _IVO_RelCabec.idCabecDoc=CabecDoc.id) left join Artigo on Artigo.Artigo=LinhasDoc.Artigo) WHERE NOT EXISTS (SELECT * FROM _IVO_RelLinhas WHERE _IVO_RelLinhas.idLinhasDoc = LinhasDoc.Id) and Artigo.Artigo is not null and _IVO_RelCabec.idEncomendaEmbal is not null and LinhasDoc.TipoLinha not like '65'";
            cmdPrimavera = new SqlCommand(strSql, cnnPrimavera1);
            int num = 0;
            if (cnnPrimavera1.State == ConnectionState.Closed)
            {
                cnnPrimavera1.Open();
            }
            num = (int)cmdPrimavera.ExecuteScalar();
            SetControlPropertyValue(lblStatus, "Text", "A sincronizar " + num.ToString() + " linhas.");
            
            int num2 = 1;
            int numLinha = 1;
            string str = "";
            cnnPrimavera1.Close();
            strSql = "SELECT LinhasDoc.Id, LinhasDoc.IdCabecDoc, _IVO_RelCabec.idEncomendaEmbal, Artigo.Artigo, "+
                @"Artigo.CDU_REFANTIGA, Artigo.CodBarras, LinhasDoc.Quantidade, "+
                @"(SELECT TOP 1 ArtigoCliente.ReferenciaCli From ArtigoCliente WHERE ArtigoCliente.Cliente=CabecDoc.Entidade), "+
                @"Artigo.TipoComponente "+ //Novo campo para determinar se é conjunto. Valor de conjunto "2"
                @"FROM (((LinhasDoc left join CabecDoc on LinhasDoc.IdCabecDoc=CabecDoc.Id) "+
                @"left join _IVO_RelCabec on _IVO_RelCabec.idCabecDoc=CabecDoc.id) "+
                @"left join Artigo on Artigo.Artigo=LinhasDoc.Artigo) "+
                @"WHERE NOT EXISTS (SELECT * FROM _IVO_RelLinhas WHERE _IVO_RelLinhas.idLinhasDoc = LinhasDoc.Id) "+
                @"and Artigo.Artigo is not null and _IVO_RelCabec.idEncomendaEmbal is not null "+
                @"and LinhasDoc.TipoLinha not like '65' ORDER BY _IVO_RelCabec.idEncomendaEmbal";

            cmdPrimavera = new SqlCommand(strSql, cnnPrimavera1);
            if (cnnPrimavera1.State == ConnectionState.Closed)cnnPrimavera1.Open();
            drPrimavera = cmdPrimavera.ExecuteReader();
            if (drPrimavera.HasRows)
            {
                while (drPrimavera.Read())
                {
                    string TipoComponente = drPrimavera[8].ToString();
                    string idLinha = drPrimavera[0].ToString();
                    string idCabec = drPrimavera[1].ToString();
                    string idEnc = drPrimavera[2].ToString();
                    string quant = drPrimavera[6].ToString();
                    string refIVO = "";
                    refIVO = drPrimavera[4].ToString() ?? "";
                    string Artigo = drPrimavera[3].ToString();
                    string UPC = "";
                    string cor = "";
                    string marca = "";
                    string espessura = "";
                    string refCliente = "";

                    if (TipoComponente == "2") //É conjunto
                    {
                        //Loop em conjuntos
                        GetArtigosConjunto(Artigo, Convert.ToInt32(quant), idLinha, idCabec, idEnc);
                    }
                    else // Não é conjunto
                    {

                        if (refIVO == "")
                        {
                            refIVO = "NÃO DEFINIDO";
                        }

                        UPC = drPrimavera[5].ToString();
                        cor = "";
                        marca = "";
                        espessura = "";
                        refCliente = drPrimavera[7].ToString();

                        if (CountChars(Artigo) == 24)
                        {
                            string[] strArray = Artigo.Split(new char[] { '.' });
                            cor = strArray[4];
                            marca = strArray[5];
                            espessura = strArray[3];
                        }

                        if (idEnc == str)
                        {
                            numLinha++;
                        }
                        else
                        {
                            numLinha = 1;
                        }

                        WriteAccessLinha(idLinha, idCabec, idEnc, quant, refIVO, Artigo, UPC, marca, cor, espessura, refCliente, numLinha.ToString());
                        worker.ReportProgress((int)((((double)num2) / ((double)num)) * 100.0));
                        num2++;
                        str = idEnc;
                    }
                }
            }
            drPrimavera.Close();
            cnnPrimavera1.Close();
            //SetControlPropertyValue(lblLinhas, "Text", num.ToString() + " linhas de encomenda sincronizadas.");
            SetControlPropertyValue(lblStatus, "Text", "Sincronização completa!");
        }

        private void GetArtigosConjunto(string ArtigoComposto, int QuantConjunto, string idLinha, string idCabec, string idEnc)
        {
            string ComponenteTipoComponente = "";
            string Componente = "";
            string QuantComponente = "";
            int iQuantComponente = 0;
            string conjuntoSQL = "SELECT C.ArtigoComposto, C.Componente, C.Quantidade, C.Ordem, A.CodBarras, AC.ReferenciaCli, A.TipoComponente, A.CDU_REFANTIGA " +
                            @"FROM (ComponentesArtigos as C left join Artigo as A on A.Artigo=C.Componente) " +
                            @"LEFT JOIN ArtigoCliente as AC on AC.Artigo=C.Componente " +
                            @"WHERE C.ArtigoComposto='" + ArtigoComposto + "' ORDER by C.Ordem";

            cmdConjunto = new SqlCommand(conjuntoSQL, cnnConjunto);
            if (cnnConjunto.State == ConnectionState.Closed) cnnConjunto.Open();
            drConjunto = cmdConjunto.ExecuteReader();

            if (drConjunto.HasRows)
            {
                while (drConjunto.Read())
                {
                    ComponenteTipoComponente = drConjunto[6].ToString();
                    Componente = drConjunto[1].ToString();
                    QuantComponente = drConjunto[2].ToString();
                    iQuantComponente = Convert.ToInt32(QuantConjunto) * Convert.ToInt32(QuantComponente);

                    if (ComponenteTipoComponente == "2")
                    {
                        break;
                    }
                    else
                    {
                        ComponenteTipoComponente = "0";
                        string cor, marca, espessura;
                        cor = "";
                        marca = "";
                        espessura = "";

                        if (CountChars(Componente) == 24)
                        {
                            string[] strArray = Componente.Split(new char[] { '.' });
                            cor = strArray[4];
                            marca = strArray[5];
                            espessura = strArray[3];
                        }

                        string refCliente = drConjunto[5].ToString();
                        string UPC = drConjunto[4].ToString();
                        string refIVO = drConjunto[7].ToString();

                        WriteAccessConjunto(idLinha, idCabec, idEnc, iQuantComponente.ToString(), refIVO, Componente, UPC, marca, cor, espessura, refCliente, numLinha.ToString());
                        numLinha++;
                        conjunto++;
                    }
                }
            }
            drConjunto.Close();
            cnnConjunto.Close();

            if (ComponenteTipoComponente == "2")
            {
                GetArtigosConjunto(Componente, iQuantComponente, idLinha, idCabec, idEnc);
            }
        }

        private void WriteAccessCabec(string Cliente, string NumDoc, string NumDocCliente, string DataSaida, string idCabecDoc)
        {
            strSql = "INSERT INTO Encomendas (Cliente_ID, Num_Enc_Ivo, Num_Enc_Cliente, Data_Saida) VALUES ('" + Cliente + "', '" + NumDoc + "', '" + NumDocCliente + "', '" + DataSaida + "')";
            if (cnnAccess.State == ConnectionState.Closed)
            {
                cnnAccess.Open();
            }
            cmdAccess = new OleDbCommand(strSql, cnnAccess);
            try
            {
                cmdAccess.ExecuteNonQuery();
                cnnAccess.Close();
            }
            catch (OleDbException exception)
            {
                MessageBox.Show(exception.ErrorCode + "\n" + exception.Message);
            }
            string idEncomenda = "";
            strSql = "SELECT top 1 id FROM Encomendas ORDER BY id DESC";
            cmdAccess = new OleDbCommand(strSql, cnnAccess);
            if (cnnAccess.State == ConnectionState.Closed)
            {
                cnnAccess.Open();
            }
            drAccess = cmdAccess.ExecuteReader();
            if (drAccess.HasRows)
            {
                while (drAccess.Read())
                {
                    idEncomenda = drAccess[0].ToString();
                }
            }
            drAccess.Close();
            cnnAccess.Close();
            WriteRelCabecPrimavera(idCabecDoc, idEncomenda);
        }

        private void WriteAccessLinha(string idLinha, string idCabec, string idEnc, string Quant, string RefIVO, string Artigo, string UPC, string Marca, string Cor, string Espessura, string RefCliente, string eln)
        {
            strSql = "INSERT INTO Ln_Enc (Enc_ID, Quantidade, Ref_Com, Marca, Cor_cabo, Espessura, Artigo, Ref_Cliente, eln, UPC) VALUES (" + idEnc + ", " + Quant + ", '" + RefIVO + "', '" + Marca + "', '" + Cor + "', '" + Espessura + "', '" + Artigo + "', '" + RefCliente + "', '" + eln + "', '" + UPC + "')";
            cmdAccess = new OleDbCommand(strSql, cnnAccess);
            if (cnnAccess.State == ConnectionState.Closed)
            {
                cnnAccess.Open();
            }
            try
            {
                cmdAccess.ExecuteNonQuery();
                cnnAccess.Close();
            }
            catch (OleDbException exception)
            {
                MessageBox.Show(exception.Message);
            }
            string str = "";
            strSql = "SELECT top 1 id FROM Ln_Enc ORDER BY id DESC";
            cmdAccess = new OleDbCommand(strSql, cnnAccess);
            if (cnnAccess.State == ConnectionState.Closed)
            {
                cnnAccess.Open();
            }
            drAccess = cmdAccess.ExecuteReader();
            if (drAccess.HasRows)
            {
                while (drAccess.Read())
                {
                    str = drAccess[0].ToString();
                }
            }
            drAccess.Close();
            cnnAccess.Close();
            WriteRelLinhasPrimavera(idLinha, str);
        }

        private void WriteAccessConjunto(string idLinha, string idCabec, string idEnc, string Quant, string RefIVO, string Artigo, string UPC, string Marca, string Cor, string Espessura, string RefCliente, string eln)
        {
            strSql = "INSERT INTO Ln_Enc (Enc_ID, Quantidade, Ref_Com, Marca, Cor_cabo, Espessura, Artigo, Ref_Cliente, eln, UPC) VALUES (" + idEnc + ", " + Quant + ", '" + RefIVO + "', '" + Marca + "', '" + Cor + "', '" + Espessura + "', '" + Artigo + "', '" + RefCliente + "', '" + eln + "', '" + UPC + "')";
            cmdAccess = new OleDbCommand(strSql, cnnAccess);
            if (cnnAccess.State == ConnectionState.Closed)
            {
                cnnAccess.Open();
            }
            try
            {
                cmdAccess.ExecuteNonQuery();
                cnnAccess.Close();
            }
            catch (OleDbException exception)
            {
                MessageBox.Show(exception.Message);
            }
            
            string str = "";
            strSql = "SELECT top 1 id FROM Ln_Enc ORDER BY id DESC";
            cmdAccess = new OleDbCommand(strSql, cnnAccess);
            if (cnnAccess.State == ConnectionState.Closed)
            {
                cnnAccess.Open();
            }
            drAccess = cmdAccess.ExecuteReader();
            if (drAccess.HasRows)
            {
                while (drAccess.Read())
                {
                    str = drAccess[0].ToString();
                }
            }
            drAccess.Close();
            cnnAccess.Close();
            WriteRelLinhasPrimavera(idLinha, str);
        }

        private void WriteRelCabecPrimavera(string idCabecDoc, string idEncomenda)
        {
            strSql = "INSERT INTO _IVO_RelCabec (idCabecDoc, idEncomendaEmbal) VALUES ('" + idCabecDoc + "', '" + idEncomenda + "')";
            cmdPrimaveraWrite = new SqlCommand(strSql, cnnPrimavera2);
            if (cnnPrimavera2.State == ConnectionState.Closed)
            {
                cnnPrimavera2.Open();
            }
            try
            {
                cmdPrimaveraWrite.ExecuteNonQuery();
                cnnPrimavera2.Close();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.ErrorCode + "\n" + exception.Message);
            }
        }

        private void WriteRelLinhasPrimavera(string idLinhasDoc, string idLn_Enc)
        {
            strSql = "INSERT INTO _IVO_RelLinhas (idLinhasDoc, idLn_Enc) VALUES ('" + idLinhasDoc + "', '" + idLn_Enc + "')";
            cmdPrimaveraWrite = new SqlCommand(strSql, cnnPrimavera2);
            if (cnnPrimavera2.State == ConnectionState.Closed)
            {
                cnnPrimavera2.Open();
            }
            try
            {
                cmdPrimaveraWrite.ExecuteNonQuery();
                cnnPrimavera2.Close();
            }
            catch (SqlException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private static int CountChars(string value)
        {
            int num = 0;
            bool flag = false;
            foreach (char ch in value)
            {
                if (char.IsWhiteSpace(ch))
                {
                    if (!flag)
                    {
                        num++;
                    }
                    flag = true;
                }
                else
                {
                    num++;
                    flag = false;
                }
            }
            return num;
        }

        private void DoBackGroundWork()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            worker.DoWork += new DoWorkEventHandler(DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleted);
            worker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            worker = sender as BackgroundWorker;
            int num = 20;
            bool flag = false;
            bool flag2 = false;
            SetControlPropertyValue(lblStatus, "Text", "A iniciar sincronização...");
            SetControlPropertyValue(lblCabecs, "Text", "");
            SetControlPropertyValue(lblLinhas, "Text", "");
            for (int i = 1; i < num; i++)
            {
                switch (i)
                {
                    case 10:
                        try
                        {
                            if (cnnPrimavera1.State == ConnectionState.Closed)
                            {
                                cnnPrimavera1.Open();
                            }
                            cnnPrimavera1.Close();
                            SetControlPropertyValue(lblCabecs, "Text", "Ligação SQL: OK!");
                            flag = true;
                        }
                        catch (SqlException exception)
                        {
                            SetControlPropertyValue(lblCabecs, "Text", "Ligação SQL: FALHA! Cod.:" + exception.ErrorCode);
                            btnSync.Enabled = false;
                        }
                        break;

                    case 0x13:
                        try
                        {
                            if (cnnAccess.State == ConnectionState.Closed)
                            {
                                cnnAccess.Open();
                            }
                            cnnAccess.Close();
                            SetControlPropertyValue(lblLinhas, "Text", "Ligação Access: OK!");
                            flag2 = true;
                        }
                        catch (OleDbException exception2)
                        {
                            SetControlPropertyValue(lblLinhas, "Text", "Ligação Access: FALHA! Cod.:" + exception2.Errors);
                            btnSync.Enabled = false;
                        }
                        break;
                }
                Thread.Sleep(180);
                worker.ReportProgress((int)((((double)i) / ((double)num)) * 100.0));
            }
            if (flag && flag2)
            {
                worker.ReportProgress(0);
                CompareCabecDoc();
            }
            else
            {
                worker.ReportProgress(100);
                MessageBox.Show("Existe um erro nas ligações às bases de dados", "Erro na ligação", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                btnSync.Enabled = false;
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _backgroundProgressBar.Value = e.ProgressPercentage;
        }

        private void SetControlPropertyValue(Control oControl, string propName, object propValue)
        {
            if (oControl.InvokeRequired)
            {
                SetControlValueCallback method = new SetControlValueCallback(SetControlPropertyValue);
                oControl.Invoke(method, new object[] { oControl, propName, propValue });
            }
            else
            {
                PropertyInfo[] properties = oControl.GetType().GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    if (info.Name.ToUpper() == propName.ToUpper())
                    {
                        info.SetValue(oControl, propValue, null);
                    }
                }
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnSync.Enabled = true;
        }

        // Nested Types
        private delegate void SetControlValueCallback(Control oControl, string propName, object propValue);

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
