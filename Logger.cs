using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ALFeBAHelper;
using eBAControls;

namespace ALFInfoLogHelper
{
	public class Logger
	{
		public static void FillForm(List<KeyValuePair<string, string>> QueryDetails, string personnelFormId, string TableName, TextBox txtLogonLang,
			eBATable tblLog, List<Control> controls)
		{   //txtLogonLang,txtOldFormId,tblLog kullanılan ebacontroller
			string Language = txtLogonLang.Text;
			List<KeyValuePair<string, string>> qrParameters = new List<KeyValuePair<string, string>>();
			qrParameters.Add(new KeyValuePair<string, string>("PERSONNELFORMID", personnelFormId));
			qrParameters.Add(new KeyValuePair<string, string>("FormName", TableName));
			DataTable dtForm = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[0].Key, QueryDetails[0].Value, qrParameters);//bir önceki dolu modal formun verileri
			if (dtForm.Rows.Count <= 0) { return; }
			foreach (Control control in controls)
			{

				if (control is eBARichTextEditor eBARichTextEditor)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						eBARichTextEditor.Text = dtForm.Rows[0][eBARichTextEditor.ID.Split('_')[0]].ToString();
					}
					catch { }
				}
				else if (control is CheckBox CheckBox)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						CheckBox.Checked = dtForm.Rows[0][CheckBox.ID.Split('_')[0]].ToString() == "1";
					}
					catch { }

				}
				else if (control is RadioButton RadioButton)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						RadioButton.Checked = dtForm.Rows[0][RadioButton.ID.Split('_')[0]].ToString() == "1";
					}
					catch { }

				}
				else if (control is RadioButtonList RadioButtonList)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						RadioButtonList.SelectedIndex = RadioButtonList.Items.IndexOf(RadioButtonList.Items.FindByValue(dtForm.Rows[0][RadioButtonList.ID.Split('_')[0]].ToString()));
					}
					catch { }

				}
				else if (control is TextBox TextBox)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						TextBox.Text = dtForm.Rows[0][TextBox.ID.Split('_')[0]].ToString();
					}
					catch { }
				}
				else if (control is eBAComboBox eBaComboBox)
				{
					try//Boş veya kullanılmayan elemanlar varsa
					{
						eBaComboBox.Value = dtForm.Rows[0][eBaComboBox.ID.Split('_')[0]].ToString();
						eBaComboBox.Text = dtForm.Rows[0][eBaComboBox.ID.Split('_')[0] + "_Text"].ToString();
					}
					catch { }
				}
				else if (control is DropDownList DropDownList)
				{
					try//Boş veya kullanılmayan elemanlar varsa
					{
						DropDownList.SelectedIndex = DropDownList.Items.IndexOf(DropDownList.Items.FindByValue(dtForm.Rows[0][DropDownList.ID.Split('_')[0]].ToString()));
						DropDownList.Text = dtForm.Rows[0][DropDownList.ID.Split('_')[0] + "_Text"].ToString();
					}
					catch { }
				}
				else if (control is eBADateTimeBox eBADateTimeBox)
				{
					try//Boş veya kullanılmayan elemanlar varsa
					{
						eBADateTimeBox.Value = DateTime.Parse(dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]].ToString());
					}
					catch { eBADateTimeBox.Clear(); }
				}
			}
			txtLogonLang.Text = Language;
			//Tablei formlar arasında aktarmak için			
			RefreshLogTable(txtLogonLang.Text, tblLog, QueryDetails[1].Key, QueryDetails[1].Value);
		}
		public  void FillForm(List<KeyValuePair<string, string>> QueryDetails, string personnelFormId, string TableName, TextBox txtLogonLang, 
			eBATable tblLog, List<Control> controls, Dictionary<string,Type> dGridInfo)
		{   //txtLogonLang,txtOldFormId,tblLog kullanılan ebacontroller
			string Language = txtLogonLang.Text;
			List<KeyValuePair<string, string>> qrParameters = new List<KeyValuePair<string, string>>();
			qrParameters.Add(new KeyValuePair<string, string>("PERSONNELFORMID", personnelFormId));
			qrParameters.Add(new KeyValuePair<string, string>("FormName", TableName));
			DataTable dtForm = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[0].Key, QueryDetails[0].Value, qrParameters);//bir önceki dolu modal formun verileri
			if (dtForm.Rows.Count <= 0) { return; }
			foreach (Control control in controls)
			{

				if (control is eBARichTextEditor eBARichTextEditor)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						eBARichTextEditor.Text = dtForm.Rows[0][eBARichTextEditor.ID.Split('_')[0]].ToString();
					}
					catch { }
				}
				else if(control is eBADetailsGrid eBADetailsGrid)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						qrParameters = new List<KeyValuePair<string, string>>();
						qrParameters.Add(new KeyValuePair<string, string>("FORMID", dtForm.Rows[0]["ID"].ToString()));
							qrParameters.Add(new KeyValuePair<string, string>("CONTROLTABLENAME", TableName+"_"+eBADetailsGrid.ID));
						qrParameters.Add(new KeyValuePair<string, string>("ORDER", "ORDER BY ORDERID ASC"));
						DataTable dtdGrid= ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[2].Key, QueryDetails[2].Value, qrParameters);
							eBADetailsGrid.CurrentRowCount = dtdGrid.Rows.Count;
							for(int i=0;i< eBADetailsGrid.CurrentRowCount;i++) 
							{
								foreach (DataColumn cl in dtdGrid.Columns)
								{
									if (cl.ColumnName != "FORMID" && cl.ColumnName != "ORDERID" && !cl.ColumnName.EndsWith("_TEXT"))
									{
										switch (dGridInfo[cl.ColumnName].Name)//tuple 3. itemi details grid icindeki nesnelerin stringlerini ve onlarin datatypelarini tutuyor, boylece ilgili cast islemlerini yapabilecegiz
										{
											case nameof(System.Web.UI.WebControls.TextBox):
												((System.Web.UI.WebControls.TextBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Text = dtdGrid.Rows[i][cl.ColumnName].ToString();
												break;
											case nameof(System.Web.UI.WebControls.CheckBox):
												((System.Web.UI.WebControls.CheckBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Checked = dtdGrid.Rows[i][cl.ColumnName].ToString() == "1";
												break;
											case nameof(System.Web.UI.WebControls.DropDownList):
												((System.Web.UI.WebControls.DropDownList)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).SelectedIndex = ((System.Web.UI.WebControls.DropDownList)eBADetailsGrid.GetRowObject(i, cl.ColumnName))
													.Items.IndexOf(((System.Web.UI.WebControls.DropDownList)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Items.FindByValue(dtForm.Rows[0][cl.ColumnName].ToString()));
												((System.Web.UI.WebControls.DropDownList)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Text = dtdGrid.Rows[i][cl.ColumnName+"_TEXT"].ToString();
												break;
											case nameof(System.Web.UI.WebControls.RadioButton):
												((System.Web.UI.WebControls.RadioButton)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Checked = dtdGrid.Rows[i][cl.ColumnName].ToString() == "1";
												break;
											case nameof(eBAControls.eBAComboBox):
												((eBAControls.eBAComboBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Text = dtdGrid.Rows[i][cl.ColumnName + "_TEXT"].ToString();
												((eBAControls.eBAComboBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Value = dtdGrid.Rows[i][cl.ColumnName].ToString();
												break;
											case nameof(eBAControls.eBADateTimeBox):
												try
												{
													((eBAControls.eBADateTimeBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName)).Value = (DateTime)dtdGrid.Rows[i][cl.ColumnName];
												}
												catch { };
												break;
										}
									}

								}
							}
						
					}
					catch { }
				}
				else if(control is CheckBox CheckBox)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						CheckBox.Checked = dtForm.Rows[0][CheckBox.ID.Split('_')[0]].ToString() == "1";
					}
					catch { }

				}
				else if(control is CheckBoxList CheckBoxList)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						qrParameters = new List<KeyValuePair<string, string>>();
						qrParameters.Add(new KeyValuePair<string, string>("FORMID", dtForm.Rows[0]["ID"].ToString()));
						qrParameters.Add(new KeyValuePair<string, string>("CONTROLTABLENAME", TableName + "_" + CheckBoxList.ID));
						qrParameters.Add(new KeyValuePair<string, string>("ORDER", ""));
						DataTable dtListBox = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[2].Key, QueryDetails[2].Value, qrParameters);
						HashSet<string> hsCheckBoxList = new HashSet<string>();
						foreach (DataRow dr in dtListBox.Rows)
						{
							hsCheckBoxList.Add(dr["VALUE"].ToString());
						}
						foreach (ListItem item in CheckBoxList.Items)
						{
							item.Selected = hsCheckBoxList.Contains(item.Value);
						}
						//CheckBoxList.SelectedIndex = CheckBoxList.Items.IndexOf(CheckBoxList.Items.FindByValue(dtForm.Rows[0][CheckBoxList.ID].ToString())); 
					}
					catch { }

				}
				else if(control is RadioButton RadioButton)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						RadioButton.Checked = dtForm.Rows[0][RadioButton.ID.Split('_')[0]].ToString() == "1";
					}
					catch { }

				}
				else if(control is RadioButtonList RadioButtonList)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						RadioButtonList.SelectedIndex = RadioButtonList.Items.IndexOf(RadioButtonList.Items.FindByValue(dtForm.Rows[0][RadioButtonList.ID.Split('_')[0]].ToString())); 
					}
					catch { }

				}
				else if(control is ListBox ListBox)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						qrParameters = new List<KeyValuePair<string, string>>();
						qrParameters.Add(new KeyValuePair<string, string>("FORMID", dtForm.Rows[0]["ID"].ToString()));
							qrParameters.Add(new KeyValuePair<string, string>("CONTROLTABLENAME", TableName + "_" + ListBox.ID));
						qrParameters.Add(new KeyValuePair<string, string>("ORDER", ""));
						DataTable dtListBox= ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[2].Key, QueryDetails[2].Value, qrParameters);
							HashSet<string> hsListBox = new HashSet<string>();
							foreach (DataRow dr in dtListBox.Rows)
							{
								hsListBox.Add(dr["VALUE"].ToString());
							}
							foreach (ListItem item in ListBox.Items)
							{
									item.Selected = hsListBox.Contains(item.Value);
							}
						
					}
					catch { }

				}
				else if(control is TextBox TextBox)
				{
					try //Boş veya kullanılmayan elemanlar varsa
					{
						TextBox.Text = dtForm.Rows[0][TextBox.ID.Split('_')[0]].ToString();
					}
					catch { }
				}
				else if(control is eBAComboBox eBaComboBox)
				{
					try//Boş veya kullanılmayan elemanlar varsa
					{
						eBaComboBox.Value = dtForm.Rows[0][eBaComboBox.ID.Split('_')[0]].ToString();
						eBaComboBox.Text = dtForm.Rows[0][eBaComboBox.ID.Split('_')[0] + "_Text"].ToString();
					}
					catch { }
				}
				else if(control is DropDownList DropDownList)
				{
					try//Boş veya kullanılmayan elemanlar varsa
					{
						DropDownList.SelectedIndex = DropDownList.Items.IndexOf(DropDownList.Items.FindByValue(dtForm.Rows[0][DropDownList.ID.Split('_')[0]].ToString()));
						DropDownList.Text = dtForm.Rows[0][DropDownList.ID.Split('_')[0] + "_Text"].ToString();
					}
					catch { }
				}
				else if(control is eBADateTimeBox eBADateTimeBox)
				{
					try//Boş veya kullanılmayan elemanlar varsa
					{
						eBADateTimeBox.Value = DateTime.Parse(dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]].ToString());
					}
					catch { eBADateTimeBox.Clear(); }
				}
			}
			txtLogonLang.Text = Language;
			//Tablei formlar arasında aktarmak için			
			RefreshLogTable(txtLogonLang.Text, tblLog, QueryDetails[1].Key, QueryDetails[1].Value);
		}

		public static void RefreshLogTable(string Language, eBATable tblLog, string connectionName, string queryName)//dinamik olarak log tablosunda multilanguage alanların dilini değiştirmek için
		{
			foreach (DataRow row in tblLog.Data.Rows)
			{
				List<string> comboList = new List<string>();
				List<string> valueList = new List<string>();
				foreach (DataColumn column in tblLog.Data.Columns)
				{
					if (column.ColumnName.Substring(0, 3) == "mtl" && !column.ColumnName.EndsWith("_Text") && !string.IsNullOrEmpty(row[column.ColumnName].ToString()))
					//mtl ile başlayan ve text olmayan comboboxları almak için boşlar queryde hata döneceğinden boşlar da alınmaz
					{
						comboList.Add(column.ColumnName);
						valueList.Add(row[column.ColumnName].ToString());
					}
				}
				string comboBox = string.Join<string>(",", comboList);
				string comboValue = string.Join<string>(",", valueList);
				if (comboBox.Length < 5) { continue; }

				DataTable dataTable = ALFIntegrationHelper.ExecuteIntegrationQuery(connectionName, queryName, new List<KeyValuePair<string, string>>()
							{
							   new KeyValuePair<string, string>("VALUEIDS",comboValue), //Replace(",","'','" )
							   new KeyValuePair<string, string>("LANG", Language)
							});
				//ALFDebugHelper.Log(667, comboBox, comboValue);
				for (int i = 0; i < comboList.Count; i++)
				{
					try
					{
						row[comboList[i] + "_Text"] = dataTable.Select("ID=" + valueList[i])[0]["value"];
					}
					catch { }
				}
			}
		}

		public void FillLogTable(List<KeyValuePair<string, string>> QueryDetails,
			string TableName, eBATable tblLog, TextBox txtPersonnelFormId, List<Control> controls, string LogonUser, string lastFormId, Dictionary<string, Type> dGridInfo, Dictionary<string, string> objectNames)
		{
			string FormName = TableName.Split('_')[TableName.Split('_').Count() - 1];
			string project = TableName.Split('_')[1];
			List<KeyValuePair<string, string>> qrParameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("ID", lastFormId),
				new KeyValuePair<string, string>("FormName", TableName)
			};
			DataTable dtForm = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[0].Key, QueryDetails[0].Value, qrParameters);//bir önceki dolu modal formun verileri
			if (dtForm.Rows.Count <= 0) { return; }
			//tblLog,txtPersonnelID kullanılan eba controlleri
			DataTable logTable = tblLog.Data;
			DataRow logrow = logTable.NewRow();
			DataRow lastrow = logrow;//yapılan değişiklikleri yakalamak için bir önceki rowun verilerini de tutmak lazım
			if (tblLog.Data.Rows.Count > 0)
			{
				lastrow = tblLog.Data.Rows[0];
			}
			List<Control> formControls = new List<Control>();
			foreach (Control control in controls)
			{
				//controllerin içinde aynı zamanda labellar ve butonlar da var ama kolon isimlerini aldığımız tabloda onlar yok
				//tablodan verileri düzgün sırayla çekmek için sadece doldurulan alanların olduğu bir liste oluşturulmalı
				if (control is TextBox ||
					control is eBAComboBox ||
					control is eBADateTimeBox ||
					control is CheckBox ||
					control is DropDownList ||
					//control is ListBox ||
					//control is eBADetailsGrid ||
					control is CheckBoxList ||
					control is RadioButton ||
					control is RadioButtonList ||
					control is eBARichTextEditor)
				{
					formControls.Add(control);
				}
			}
			var controlIds = formControls.Where(x => !x.ID.Contains("_")).Select(x => x.ID).Distinct().ToList();//controllerin isimleri
			controlIds.Sort();
			string controlID = string.Join<string>("','", controlIds);
			DataTable dataTable = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[1].Key, QueryDetails[1].Value, new List<KeyValuePair<string, string>>()
							{
							   new KeyValuePair<string, string>("ControlID",controlID),
							   new KeyValuePair<string, string>("FormName",FormName),
							   new KeyValuePair<string, string>("PROJECT",project)
							});
			Dictionary<string, string> labelDict = new Dictionary<string, string>();
			int j = 0;
			for (int i = 0; i < controlIds.Count(); i++)
			{
				if (!controlIds[i].StartsWith("Label") && !controlIds[i].StartsWith("btn"))
				{
					labelDict.Add(controlIds[i], dataTable.Rows[j][0].ToString());//controller ile labellar key value pair oluşturuluyor
					j++;
				}
			}
			logrow["PersonnelFormId"] = txtPersonnelFormId.Text;
			foreach (Control control in controls)
			{
				if (control is eBARichTextEditor)
				{
					eBARichTextEditor eBARichTextEditor = (eBARichTextEditor)control;
					try
					{
						logrow[eBARichTextEditor.ID.Split('_')[0]] = dtForm.Rows[0][eBARichTextEditor.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if (dtForm.Rows[0][eBARichTextEditor.ID.Split('_')[0]].ToString() != eBARichTextEditor.Text && eBARichTextEditor.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBARichTextEditor.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is eBADetailsGrid)
				{
					eBADetailsGrid eBADetailsGrid = (eBADetailsGrid)control;
					qrParameters = new List<KeyValuePair<string, string>>();
					qrParameters.Add(new KeyValuePair<string, string>("FORMID", dtForm.Rows[0]["ID"].ToString()));
					qrParameters.Add(new KeyValuePair<string, string>("CONTROLTABLENAME", TableName + "_" + eBADetailsGrid.ID));
					qrParameters.Add(new KeyValuePair<string, string>("ORDER", "ORDER BY ORDERID ASC"));
					DataTable dtdGrid = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[2].Key, QueryDetails[2].Value, qrParameters);
					eBADetailsGrid.CurrentRowCount = dtdGrid.Rows.Count;
					for (int i = 0; i < eBADetailsGrid.CurrentRowCount; i++)
					{
						foreach (DataColumn cl in dtdGrid.Columns)
						{
							if (cl.ColumnName != "FORMID" && cl.ColumnName != "ORDERID" && !cl.ColumnName.EndsWith("_TEXT"))
							{
								switch (dGridInfo[cl.ColumnName].Name)//tuple 3. itemi details grid icindeki nesnelerin stringlerini ve onlarin datatypelarini tutuyor, boylece ilgili cast islemlerini yapabilecegiz
								{
									case "TextBox":
										TextBox textBoxDG = ((System.Web.UI.WebControls.TextBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName));
										try
										{
											if ((dtdGrid.Rows[i][textBoxDG.ID.Split('_')[0]].ToString() != textBoxDG.Text && textBoxDG.ID.StartsWith("txt")) && textBoxDG.Visible == true)
											{
												logrow["ChangedFields"] += Environment.NewLine + objectNames[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
										}
										catch { }
										break;
									case "CheckBox":
										CheckBox checkBoxDG = ((System.Web.UI.WebControls.CheckBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName));
										try
										{
											if ((dtdGrid.Rows[i][checkBoxDG.ID.Split('_')[0]].ToString() == "1") != checkBoxDG.Checked && checkBoxDG.Visible == true)
											{
												logrow["ChangedFields"] += Environment.NewLine + objectNames[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
										}
										catch (Exception ex)
										{
											ALFDebugHelper.Log(13, dtForm.Columns);
											throw ex;
										}
										break;
									case "DropDownList":
										DropDownList dropDownListDG = ((System.Web.UI.WebControls.DropDownList)eBADetailsGrid.GetRowObject(i, cl.ColumnName));
										try
										{
											if ((dtdGrid.Rows[i][dropDownListDG.ID.Split('_')[0]].ToString() != dropDownListDG.SelectedItem.Value) && dropDownListDG.Visible == true)
											{
												logrow["ChangedFields"] += Environment.NewLine + objectNames[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
										}
										catch { }
										break;
									case "RadioButton":
										RadioButton radioButtonDG = ((System.Web.UI.WebControls.RadioButton)eBADetailsGrid.GetRowObject(i, cl.ColumnName));
										try
										{
											if ((dtdGrid.Rows[i][radioButtonDG.ID.Split('_')[0]].ToString() == "1") != radioButtonDG.Checked && radioButtonDG.Visible == true)
											{
												logrow["ChangedFields"] += Environment.NewLine + objectNames[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
										}
										catch { }
										break;
									case "eBAComboBox":
										eBAComboBox comboBoxDG = ((eBAControls.eBAComboBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName));
										try
										{
											if ((dtdGrid.Rows[i][comboBoxDG.ID.Split('_')[0]].ToString() != comboBoxDG.Value) && comboBoxDG.Visible == true)
											{
												logrow["ChangedFields"] += Environment.NewLine + objectNames[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
										}
										catch { }
										break;
									case "eBADateTimeBox":

										eBADateTimeBox dateTimeBoxDG = ((eBAControls.eBADateTimeBox)eBADetailsGrid.GetRowObject(i, cl.ColumnName));

										try
										{
											if (!dateTimeBoxDG.IsValid && !string.IsNullOrEmpty(dtdGrid.Rows[i][dateTimeBoxDG.ID.Split('_')[0]].ToString()))
											{
												logrow["ChangedFields"] += Environment.NewLine + labelDict[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
											else if ((dtdGrid.Rows[i][dateTimeBoxDG.ID.Split('_')[0]].ToString() != dateTimeBoxDG.Value.ToString()) && dateTimeBoxDG.Visible == true)
											{
												logrow["ChangedFields"] += Environment.NewLine + labelDict[eBADetailsGrid.ID.Split('_')[0]].Replace(":", "--");
												goto Exit;
											}
										}
										catch { }
										break;
								}
							}

						}
					}
				Exit:;
				}
				else if (control is CheckBox)
				{
					CheckBox CheckBox = (CheckBox)control;
					try
					{
						logrow[CheckBox.ID.Split('_')[0]] = dtForm.Rows[0][CheckBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][CheckBox.ID.Split('_')[0]].ToString() == "1") != CheckBox.Checked && CheckBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[CheckBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }

				}
				else if (control is CheckBoxList)
				{
					CheckBoxList CheckBoxList = (CheckBoxList)control;
					qrParameters = new List<KeyValuePair<string, string>>();
					qrParameters.Add(new KeyValuePair<string, string>("FORMID", dtForm.Rows[0]["ID"].ToString()));
					qrParameters.Add(new KeyValuePair<string, string>("CONTROLTABLENAME", TableName + "_" + CheckBoxList.ID));
					qrParameters.Add(new KeyValuePair<string, string>("ORDER", ""));
					DataTable dtListBox = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[2].Key, QueryDetails[2].Value, qrParameters);
					HashSet<string> hsCheckBoxList = new HashSet<string>();
					HashSet<string> hsCheckBoxListSelected = new HashSet<string>();
					foreach (DataRow dr in dtListBox.Rows)
					{
						hsCheckBoxList.Add(dr["VALUE"].ToString());
					}
					foreach (ListItem item in CheckBoxList.Items)
					{
						if (item.Selected)
						{
							hsCheckBoxListSelected.Add(item.Value);
						}
						if (item.Selected && !hsCheckBoxList.Contains(item.Value))
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[CheckBoxList.ID.Split('_')[0]].Replace(":", "--");
							goto checkBoxExit;
						}
					}
					foreach (var item in hsCheckBoxList)
					{

						if (!hsCheckBoxListSelected.Contains(item))
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[CheckBoxList.ID.Split('_')[0]].Replace(":", "--");
							goto checkBoxExit;
						}
					}
				checkBoxExit:;

				}
				else if (control is RadioButton)
				{
					RadioButton RadioButton = (RadioButton)control;
					try
					{
						logrow[RadioButton.ID.Split('_')[0]] = dtForm.Rows[0][RadioButton.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][RadioButton.ID.Split('_')[0]].ToString() == "1") != RadioButton.Checked && RadioButton.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[RadioButton.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }

				}
				else if (control is RadioButtonList)
				{
					RadioButtonList RadioButtonList = (RadioButtonList)control;
					try
					{
						logrow[RadioButtonList.ID + "_Text"] = dtForm.Rows[0][RadioButtonList.ID + "_Text"];
						logrow[RadioButtonList.ID.Split('_')[0]] = dtForm.Rows[0][RadioButtonList.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][RadioButtonList.ID.Split('_')[0]].ToString() != RadioButtonList.SelectedValue) && RadioButtonList.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[RadioButtonList.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }

				}
				else if (control is ListBox)
				{

					ListBox ListBox = (ListBox)control;
					try
					{
						qrParameters = new List<KeyValuePair<string, string>>();

						qrParameters.Add(new KeyValuePair<string, string>("FORMID", dtForm.Rows[0]["ID"].ToString()));
						qrParameters.Add(new KeyValuePair<string, string>("CONTROLTABLENAME", TableName + "_" + ListBox.ID));
						qrParameters.Add(new KeyValuePair<string, string>("ORDER", ""));
						DataTable dtListBox = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[2].Key, QueryDetails[2].Value, qrParameters);
						HashSet<string> hsListBox = new HashSet<string>();
						HashSet<string> hsListBoxSelected = new HashSet<string>();
						foreach (DataRow dr in dtListBox.Rows)
						{
							hsListBox.Add(dr["VALUE"].ToString());
						}
						foreach (ListItem item in ListBox.Items)
						{
							if (item.Selected)
							{
								hsListBoxSelected.Add(item.Value);
							}
							if (item.Selected && !hsListBox.Contains(item.Value))
							{
								logrow["ChangedFields"] += Environment.NewLine + objectNames[ListBox.ID.Split('_')[0]].Replace(":", "--");
								goto ListBoxExit;
							}
						}
						foreach (var item in hsListBox)
						{

							if (!hsListBoxSelected.Contains(item))
							{
								logrow["ChangedFields"] += Environment.NewLine + objectNames[ListBox.ID.Split('_')[0]].Replace(":", "--");
								goto ListBoxExit;
							}
						}
					}
					catch (KeyNotFoundException ex)
					{
						throw new Exception(ListBox.ID.Split('_')[0]);
					}
				ListBoxExit:;
				}
				else if (control is TextBox)
				{
					TextBox TextBox = (TextBox)control;
					try
					{
						logrow[TextBox.ID.Split('_')[0]] = dtForm.Rows[0][TextBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][TextBox.ID.Split('_')[0]].ToString() != TextBox.Text && TextBox.ID.StartsWith("txt")) && TextBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[TextBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is eBAComboBox)
				{
					eBAComboBox eBaComboBox = (eBAComboBox)control;
					try
					{
						logrow[eBaComboBox.ID + "_Text"] = dtForm.Rows[0][eBaComboBox.ID + "_Text"];
						logrow[eBaComboBox.ID.Split('_')[0]] = dtForm.Rows[0][eBaComboBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][eBaComboBox.ID.Split('_')[0]].ToString() != eBaComboBox.Value) && eBaComboBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBaComboBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is DropDownList)
				{
					DropDownList DropDownList = (DropDownList)control;
					try
					{
						logrow[DropDownList.ID + "_Text"] = dtForm.Rows[0][DropDownList.ID + "_Text"];
						logrow[DropDownList.ID.Split('_')[0]] = dtForm.Rows[0][DropDownList.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][DropDownList.ID.Split('_')[0]].ToString() != DropDownList.SelectedItem.Value) && DropDownList.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[DropDownList.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is eBADateTimeBox)
				{
					eBADateTimeBox eBADateTimeBox = (eBADateTimeBox)control;
					try
					{
						logrow[eBADateTimeBox.ID.Split('_')[0]] = dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if (!eBADateTimeBox.IsValid && !string.IsNullOrEmpty(dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]].ToString()) && eBADateTimeBox.ID.StartsWith("txt"))
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBADateTimeBox.ID.Split('_')[0]].Replace(":", "--");
						}
						if ((dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]].ToString() != eBADateTimeBox.Value.ToString()) && eBADateTimeBox.Visible == true && eBADateTimeBox.ID.StartsWith("txt"))
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBADateTimeBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
			}
			logrow["LogDate"] = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
			logrow["LogUser"] = LogonUser;
			logrow["ModalFormId"] = Int32.Parse(lastFormId).ToString("D10");
			if (tblLog.Data.Rows.Count > 0 && string.IsNullOrEmpty(logrow["ChangedFields"].ToString()))
			{
			}
			else
			{
				logTable.Rows.Add(logrow);
			}
		}

		public static void FillLogTable(List<KeyValuePair<string, string>> QueryDetails,
			string TableName, eBATable tblLog, TextBox txtPersonnelFormId, List<Control> controls, string LogonUser, string lastFormId)
		{
			string FormName = TableName.Split('_')[TableName.Split('_').Count() - 1];
			string project = TableName.Split('_')[1];
			List<KeyValuePair<string, string>> qrParameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("ID", lastFormId),
				new KeyValuePair<string, string>("FormName", TableName)
			};
			DataTable dtForm = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[0].Key, QueryDetails[0].Value, qrParameters);//bir önceki dolu modal formun verileri
			if (dtForm.Rows.Count <= 0) { return; }
			//tblLog,txtPersonnelID kullanılan eba controlleri
			DataTable logTable = tblLog.Data;
			DataRow logrow = logTable.NewRow();
			DataRow lastrow = logrow;//yapılan değişiklikleri yakalamak için bir önceki rowun verilerini de tutmak lazım
			if (tblLog.Data.Rows.Count > 0)
			{
				lastrow = tblLog.Data.Rows[0];
			}
			List<Control> formControls = new List<Control>();
			foreach (Control control in controls)
			{
				//controllerin içinde aynı zamanda labellar ve butonlar da var ama kolon isimlerini aldığımız tabloda onlar yok
				//tablodan verileri düzgün sırayla çekmek için sadece doldurulan alanların olduğu bir liste oluşturulmalı
				if (control is TextBox ||
					control is eBAComboBox ||
					control is eBADateTimeBox ||
					control is CheckBox ||
					control is DropDownList ||
					control is RadioButton ||
					control is RadioButtonList ||
					control is eBARichTextEditor)
				{
					formControls.Add(control);
				}
			}
			var controlIds = formControls.Where(x => !x.ID.Contains("_")).Select(x => x.ID).Distinct().ToList();//controllerin isimleri
			controlIds.Sort();
			string controlID = string.Join<string>("','", controlIds);
			DataTable dataTable = ALFIntegrationHelper.ExecuteIntegrationQuery(QueryDetails[1].Key, QueryDetails[1].Value, new List<KeyValuePair<string, string>>()
							{
							   new KeyValuePair<string, string>("ControlID",controlID),
							   new KeyValuePair<string, string>("FormName",FormName),
							   new KeyValuePair<string, string>("PROJECT",project)
							});
			Dictionary<string, string> labelDict = new Dictionary<string, string>();
			int j = 0;
			for (int i = 0; i < controlIds.Count(); i++)
			{
				if (!controlIds[i].StartsWith("Label") && !controlIds[i].StartsWith("btn"))
				{
					labelDict.Add(controlIds[i], dataTable.Rows[j][0].ToString());//controller ile labellar key value pair oluşturuluyor
					j++;
				}
			}
			logrow["PersonnelFormId"] = txtPersonnelFormId.Text;
			foreach (Control control in controls)
			{
				if (control is eBARichTextEditor)
				{
					eBARichTextEditor eBARichTextEditor = (eBARichTextEditor)control;
					try
					{
						logrow[eBARichTextEditor.ID.Split('_')[0]] = dtForm.Rows[0][eBARichTextEditor.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if (dtForm.Rows[0][eBARichTextEditor.ID.Split('_')[0]].ToString() != eBARichTextEditor.Text && eBARichTextEditor.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBARichTextEditor.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is CheckBox)
				{
					CheckBox CheckBox = (CheckBox)control;
					try
					{
						logrow[CheckBox.ID.Split('_')[0]] = dtForm.Rows[0][CheckBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][CheckBox.ID.Split('_')[0]].ToString() == "1") != CheckBox.Checked && CheckBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[CheckBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }

				}
				else if (control is RadioButton)
				{
					RadioButton RadioButton = (RadioButton)control;
					try
					{
						logrow[RadioButton.ID.Split('_')[0]] = dtForm.Rows[0][RadioButton.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][RadioButton.ID.Split('_')[0]].ToString() == "1") != RadioButton.Checked && RadioButton.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[RadioButton.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }

				}
				else if (control is RadioButtonList)
				{
					RadioButtonList RadioButtonList = (RadioButtonList)control;
					try
					{
						logrow[RadioButtonList.ID + "_Text"] = dtForm.Rows[0][RadioButtonList.ID + "_Text"];
						logrow[RadioButtonList.ID.Split('_')[0]] = dtForm.Rows[0][RadioButtonList.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][RadioButtonList.ID.Split('_')[0]].ToString() != RadioButtonList.SelectedValue) && RadioButtonList.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[RadioButtonList.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }

				}
				else if (control is TextBox)
				{
					TextBox TextBox = (TextBox)control;
					try
					{
						logrow[TextBox.ID.Split('_')[0]] = dtForm.Rows[0][TextBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][TextBox.ID.Split('_')[0]].ToString() != TextBox.Text && TextBox.ID.StartsWith("txt")) && TextBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[TextBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is eBAComboBox)
				{
					eBAComboBox eBaComboBox = (eBAComboBox)control;
					try
					{
						logrow[eBaComboBox.ID + "_Text"] = dtForm.Rows[0][eBaComboBox.ID + "_Text"];
						logrow[eBaComboBox.ID.Split('_')[0]] = dtForm.Rows[0][eBaComboBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][eBaComboBox.ID.Split('_')[0]].ToString() != eBaComboBox.Value) && eBaComboBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBaComboBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is DropDownList)
				{
					DropDownList DropDownList = (DropDownList)control;
					try
					{
						logrow[DropDownList.ID + "_Text"] = dtForm.Rows[0][DropDownList.ID + "_Text"];
						logrow[DropDownList.ID.Split('_')[0]] = dtForm.Rows[0][DropDownList.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if ((dtForm.Rows[0][DropDownList.ID.Split('_')[0]].ToString() != DropDownList.SelectedItem.Value) && DropDownList.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[DropDownList.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
				else if (control is eBADateTimeBox)
				{
					eBADateTimeBox eBADateTimeBox = (eBADateTimeBox)control;
					try
					{
						logrow[eBADateTimeBox.ID.Split('_')[0]] = dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]];
					}
					catch { }
					try
					{
						if (!eBADateTimeBox.IsValid && !string.IsNullOrEmpty(dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]].ToString()))
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBADateTimeBox.ID.Split('_')[0]].Replace(":", "--");
						}
						if ((dtForm.Rows[0][eBADateTimeBox.ID.Split('_')[0]].ToString() != eBADateTimeBox.Value.ToString()) && eBADateTimeBox.Visible == true)
						{
							logrow["ChangedFields"] += Environment.NewLine + labelDict[eBADateTimeBox.ID.Split('_')[0]].Replace(":", "--");
						}
					}
					catch { }
				}
			}
			logrow["LogDate"] = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
			logrow["LogUser"] = LogonUser;
			logrow["ModalFormId"] = Int32.Parse(lastFormId).ToString("D10");
			if (tblLog.Data.Rows.Count > 0 && string.IsNullOrEmpty(logrow["ChangedFields"].ToString()))
			{
			}
			else
			{
				logTable.Rows.Add(logrow);
			}
		}
	}
}
