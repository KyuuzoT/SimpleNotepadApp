using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleNotepad
{
    public partial class SimpleNotepadForm : Form
    {
        private string sOpenFileName; //Переменная для хранения имени файла после диалога открытия. Нужна для использования в методе сохранения файла.
        private StringReader srReader; //Нужна для печати содержимого текстбокса
        private UInt16 iPrintPageNumber; //Хранит номер текущей распечатываемой страницы.
        private bool bDocumentChanged = false; //Флаг изменений документа.
        private StatusBar statusBar = new StatusBar();
        private StatusBarPanel promptBar = new StatusBarPanel();
        private StatusBarPanel isChangedBar = new StatusBarPanel();
        private enum eAlignment //инициализация нумератора для выбора режима выравнивания
        {
            Left,
            Right,
            Center
        };


        private eAlignment eAlign = eAlignment.Left; //переменная-нумератор для использования в switch

        public SimpleNotepadForm()
        {
            InitializeComponent();
            BuildingStatusBar();
        }

        public void BuildingStatusBar()
        {
            statusBar.ShowPanels = true;
            statusBar.Size = new System.Drawing.Size(838, 20);
            statusBar.Panels.AddRange(new StatusBarPanel[] { promptBar, isChangedBar });
            promptBar.BorderStyle = StatusBarPanelBorderStyle.None;
            promptBar.AutoSize = StatusBarPanelAutoSize.Spring;
            promptBar.Width = 648;

            isChangedBar.Width = 190;
            isChangedBar.Alignment = HorizontalAlignment.Right;
            this.Controls.Add(statusBar);
        }


        private void menuFile_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Создание нового файла. Для этого можно просто очистить поле ввода.
        /// </summary>
        private void menuFileNew_Click(object sender, EventArgs e)
        {
            if (bDocumentChanged)
            {
                DialogResult drResult = MessageBox.Show("Все несохраненные изменения будут потеряны! Хотите ли соxранить их?", "Есть несохраненные изменения", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                //Свитчим ответ на выскочившее сообщение
                switch (drResult)
                {
                    case DialogResult.Yes:
                        MenuFileSave();
                        break;
                    case DialogResult.No:
                        richTextBox1.Clear();
                        bDocumentChanged = false;
                        this.Text = ("File: [" + "new document 1" + "]");
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        break;
                }
            }
            
        }

        #region "Open File" menu
        private void menuFileOpen_Click(object sender, EventArgs e)
        {
            if (bDocumentChanged)
            {
                DialogResult drResult = MessageBox.Show("Все несохраненные изменения будут потеряны! Хотите ли соxранить их?", "Есть несохраненные изменения", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                //Свитчим ответ на выскочившее сообщение
                switch (drResult)
                {
                    case DialogResult.Yes:
                        MenuFileSave();
                        MenuFileOpen();
                        break;
                    case DialogResult.No:
                        MenuFileOpen();
                        bDocumentChanged = false;
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Метод открытия нового файла, вызывается в функции обработки нажатия строки меню "Open File..."
        /// </summary>
        private void MenuFileOpen()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileName.Length > 0)
            {
                try //Пробуем открыть файл *.rtf
                {
                    richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.RichText);
                }
                catch (ArgumentException ex) //Если ловим исключение - открываем, как простой текст
                {                           //с потерей форматирования.
                    richTextBox1.LoadFile(openFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                }
                this.Text = ("File: [" + openFileDialog1.FileName + "]"); //Выводим в заголовке окна название файла, который открывали.
                sOpenFileName = openFileDialog1.FileName;
            }
            #region Experiment
            else if (openFileDialog1.FileName.Length == 0)
            {
                MessageBox.Show("Пустое имя файла недопустимо!", "Недопустимый ввод!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MenuFileOpen();
            }
            #endregion
        }
        #endregion

        #region "Save File" menu


        /// <summary>
        /// Сохранение файла. Если мы уже открыли какой-то файл, то сохраняем без диалога, иначе вызываем метод, открывающий диалог сохранения нового файла.
        /// </summary>
        private void menuFileSave_Click(object sender, EventArgs e)
        {
            MenuFileSave();
        }

        /// <summary>
        /// Метод создания и сохранения нового файла. Вызывает диалог, для выбора пути и имени сохраняемого файла.
        /// </summary>
        private void menuFileSaveAs_Click(object sender, EventArgs e)
        {
            MenuFileSaveAs();
        }

        /// <summary>
        /// Метод для сохранения файла без диалога, если какой-либо файл уже редактируется. В противном случае откроется диалог.
        /// </summary>
        private void MenuFileSave()
        {
            if (sOpenFileName.Length > 0)
            {
                if (sOpenFileName.EndsWith("*.rtf"))
                {
                    richTextBox1.SaveFile(sOpenFileName); //Используем сохраненное ранее имя, чтобы не вызывать диалог и просто сохранить изменения, внесенные в файл.
                }
                else
                {
                    richTextBox1.SaveFile(sOpenFileName, RichTextBoxStreamType.PlainText);
                }
                bDocumentChanged = false;
            }
            else
            {
                //Если у нас не было открыто ни одного файла, то мы запускаем сохранение с диалогом.
                MenuFileSaveAs();
            }

        }

        /// <summary>
        /// Сохранение с диалогом.
        /// </summary>
        private void MenuFileSaveAs()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName.Length > 0)
            {
                if (saveFileDialog1.FileName.EndsWith("*.rtf"))
                {
                    richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.RichText);
                }
                else
                {
                    richTextBox1.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                }
                this.Text = ("File: [" + saveFileDialog1.FileName + "]");
                bDocumentChanged = false;
            }
        }


        #endregion


        private void menuFileSetup_Click(object sender, EventArgs e)
        {
            MenuFilePageSetup();
        }

        /// <summary>
        /// Настройка параметров страницы
        /// </summary>
        private void MenuFilePageSetup()
        {
            pageSetupDialog1.ShowDialog();
        }


        #region File printing
        private void menuFilePrint_Click(object sender, EventArgs e)
        {
            MenuFilePrint();
        }

        /// <summary>
        /// Метод печати файла.
        /// </summary>
        private void MenuFilePrint()
        {
            iPrintPageNumber = 1; //1 - потому что печать начинается с первой страницы
            string _sText = this.richTextBox1.Text;
            srReader = new StringReader(_sText); //Открываем поток и записываем туда печатаемый текст

            Margins _margins = new Margins(100, 50, 50, 50); //Отступы
            printDocument1.DefaultPageSettings.Margins = _margins;

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                this.printDocument1.Print();
            }

            srReader.Close();
        }

        /// <summary>
        /// Обработка события отправки на печать страницы
        /// </summary>
        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            int _iLineCount = 0; //Счетчик строк
            float _fLinesPerPage = 0; //Кол-во строк на одну страницу
            float _fLinePositionY = 0; //Текущая позиция при печати по оси ординат
            string _sCurrentLine = null; //Строка, распечатываемая в данный момент
            Font _printFont = this.richTextBox1.Font; //Шрифт для печати текста
            SolidBrush _sbPrintBrush = new SolidBrush(Color.Black); //Кисть для печати текста (задает цвет печати)
            float _fMarginLeft = e.MarginBounds.Left; //Размер отступа слева
            float _fMarginTop = e.MarginBounds.Top + (3 * _printFont.GetHeight(e.Graphics)); //размер отступа сверху
            string _sPageNumber = "Page {" + iPrintPageNumber.ToString() + "}"; //Для печати номера страницы.
            SizeF _sfStringSize = new SizeF();

            _fLinesPerPage = (e.MarginBounds.Height - 6 * _printFont.GetHeight(e.Graphics)) / _printFont.GetHeight(e.Graphics); //С учетом размеров отступа

            //Цикл печати
            for (_iLineCount = 0; _iLineCount < _fLinesPerPage; _iLineCount++)
            {
                _sCurrentLine = srReader.ReadLine();

                if (_sCurrentLine == null)
                    break;

                //Вычисляем позицию распечатываемой строки
                _fLinePositionY = _fMarginTop + (_iLineCount * _printFont.GetHeight(e.Graphics));

                e.Graphics.DrawString(_sCurrentLine, _printFont, _sbPrintBrush, _fMarginLeft, _fLinePositionY, new StringFormat()); //Сама печать
            }

            //Далее переходим к печати колонтитулов
            _sfStringSize = e.Graphics.MeasureString(_sPageNumber, _printFont, e.MarginBounds.Right - e.MarginBounds.Left);
            //Печатаем номер страницы
            e.Graphics.DrawString(_sPageNumber, _printFont, _sbPrintBrush, e.MarginBounds.Right - _sfStringSize.Width, e.MarginBounds.Top, new StringFormat());

            //Печатаем имя документа
            e.Graphics.DrawString(this.Text, _printFont, _sbPrintBrush, e.MarginBounds.Left, e.MarginBounds.Top, new StringFormat());

            //Кисть для рисования горизонтальной линии, отделяющей верхний колонтитул.
            Pen _colontitulPen = new Pen(Color.Black);
            _colontitulPen.Width = 2;

            //Рисуем горизонтальную линию, отделяющую верхний колонтитул
            e.Graphics.DrawLine(_colontitulPen, _fMarginLeft, e.MarginBounds.Top + _printFont.GetHeight(e.Graphics) + 3, e.MarginBounds.Right, e.MarginBounds.Top + _printFont.GetHeight(e.Graphics) + 3);

            string _colontitulString = System.Environment.MachineName; //Строка, печатаемая в колонтитуле
            //Печатаем строку колонтитула
            e.Graphics.DrawString(_colontitulString, _printFont, _sbPrintBrush, e.MarginBounds.Left, e.MarginBounds.Bottom, new StringFormat());

            //Если напечатаны не все строки документа, то переходим к следующей строке
            if (_sCurrentLine != null)
            {
                e.HasMorePages = true;
                iPrintPageNumber++;
            }
            else
                e.HasMorePages = false;

            _sbPrintBrush.Dispose();
            _colontitulPen.Dispose();
        }
        #endregion

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            if (bDocumentChanged)
            {
                DialogResult drResult = MessageBox.Show("Все несохраненные изменения будут потеряны! Хотите ли соxранить их?", "Есть несохраненные изменения", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                //Свитчим ответ на выскочившее сообщение
                switch (drResult)
                {
                    case DialogResult.Yes:
                        MenuFileSave();
                        Dispose(true);
                        this.Close();
                        break;
                    case DialogResult.No:
                        bDocumentChanged = false;
                        Dispose(true);
                        this.Close();
                        break;
                    case DialogResult.Cancel:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Dispose(true);
                this.Close();
            }
            
        }

        private void menuEdit_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Группа методов из меню редактир
        /// </summary>
        ///
        #region "Edit menu" realization
        

        private void menuEditUndo_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo(); //Отменить последнее действие
        }

        private void menuEditRedo_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo(); //Возвратить последнее действие
        }

        private void menuEditCut_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut(); //Вырезать выделенную строку/символ
        }

        private void menuEditCopy_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy(); //Копировать выделенное
        }

        private void menuEditPaste_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste(); //Вставить из буфера обмена
        }

        private void menuEditDelete_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut(); //Тоже функция вырезания. Такая уж реализация удаления.
        }

        private void menuEditSelectAll_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll(); //Выделить все.
        }

        #endregion




        private void menuHelp_Click(object sender, EventArgs e)
        {

        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            Form _dlgAbout = new WindowsFormsApp1.HelpAbout();
            _dlgAbout.ShowDialog();
        }
        #region Print file Preview
        private void menuFilePrintPreview_Click(object sender, EventArgs e)
        {
            MenuFilePrintPreview();
        }

        /// <summary>
        /// Метод для предварительного просмотра файла, отдающегося на печать
        /// </summary>
        private void MenuFilePrintPreview()
        {
            iPrintPageNumber = 1; //Один, а не ноль, потому что мы начинаем печатать с первой страницы
            string _sText = this.richTextBox1.Text;
            srReader = new StringReader(_sText); //Открываем поток чтения для последующей печати
            Margins _margins = new Margins(100,50,50,50); //Задаем отступы в сотых долях дюйма

            printDocument1.DefaultPageSettings.Margins = _margins;
            printPreviewDialog1.ShowDialog(); //Открываем диалог предпросмотра печатаемого.

            srReader.Close(); //Не забываем закрывать поток.
        }

        #endregion

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            bDocumentChanged = true;
            isChangedBar.Text = "Есть несохраненные изменения";
        }

        /// <summary>
        /// Запускает диалог выбора шрифта.
        /// </summary>
        private void menuFormatFont_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
                richTextBox1.SelectionFont = fontDialog1.Font;
        }

        private void menuFormatColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                richTextBox1.SelectionColor = colorDialog1.Color;
        }

        #region Text style

        /// <summary>
        /// Устанавливает полужирный шрифт по клику
        /// </summary>
        private void menuFormatStyleBold_Click(object sender, EventArgs e)
        {
            SetBold();
        }

        /// <summary>
        /// Устанавливает шрифт курсивом по клику
        /// </summary>
        private void menuFormatStyleItalic_Click(object sender, EventArgs e)
        {
            SetItalic();
        }

        /// <summary>
        /// Устанавливает подчеркнутый шрифт по клику
        /// </summary>
        private void menuFormatStyleUnderline_Click(object sender, EventArgs e)
        {
            SetUnderline();
        }

        /// <summary>
        /// Устанавливает перечеркнутый шрифт по клику
        /// </summary>
        private void menuFormatStyleStrikeOut_Click(object sender, EventArgs e)
        {
            SetStrikeout();
        }

        /// <summary>
        /// Установка полужирного стиля символов
        /// </summary>
        private void SetBold()
        {
            if (richTextBox1.SelectionFont != null)
            {
                System.Drawing.Font _curFont = richTextBox1.SelectionFont;
                System.Drawing.FontStyle _newFontStyle;

                if (richTextBox1.SelectionFont.Bold)
                {
                    _newFontStyle = FontStyle.Regular;
                }
                else
                {
                    _newFontStyle = FontStyle.Bold;
                }

                richTextBox1.SelectionFont = new Font(_curFont.FontFamily, _curFont.Size, _newFontStyle);
                CheckMenuFontCharacterStyle();
            }
        }

        /// <summary>
        /// Установка курсивного стиля символов
        /// </summary>
        private void SetItalic()
        {
            System.Drawing.Font _curFont = richTextBox1.SelectionFont;
            System.Drawing.FontStyle _newFontStyle;

            if (richTextBox1.SelectionFont.Italic)
            {
                _newFontStyle = FontStyle.Regular;
            }
            else
            {
                _newFontStyle = FontStyle.Italic;
            }
            richTextBox1.SelectionFont = new Font(_curFont.FontFamily, _curFont.Size, _newFontStyle);
            CheckMenuFontCharacterStyle();
        }

        /// <summary>
        /// Установка подчеркивания символов
        /// </summary>
        private void SetUnderline()
        {
            System.Drawing.Font _curFont = richTextBox1.SelectionFont;
            System.Drawing.FontStyle _newFontStyle;

            if (richTextBox1.SelectionFont.Underline)
            {
                _newFontStyle = FontStyle.Regular;
            }
            else
            {
                _newFontStyle = FontStyle.Underline;
            }
            richTextBox1.SelectionFont = new Font(_curFont.FontFamily, _curFont.Size, _newFontStyle);
            CheckMenuFontCharacterStyle();
        }

        /// <summary>
        /// Установка вычеркнутого стиля символов
        /// </summary>
        private void SetStrikeout()
        {
            System.Drawing.Font _curFont = richTextBox1.SelectionFont;
            System.Drawing.FontStyle _newFontStyle;

            if (richTextBox1.SelectionFont.Strikeout)
            {
                _newFontStyle = FontStyle.Regular;
            }
            else
            {
                _newFontStyle = FontStyle.Strikeout;
            }
            richTextBox1.SelectionFont = new Font(_curFont.FontFamily, _curFont.Size, _newFontStyle);
            CheckMenuFontCharacterStyle();
        }

        /// <summary>
        /// Установка галочек в меню стиля текста
        /// </summary>
        private void CheckMenuFontCharacterStyle()
        { 
            if (richTextBox1.SelectionFont.Bold)
            {
                menuFormatStyleBold.Checked = true;
            }
            else
            {
                menuFormatStyleBold.Checked = false;
            }
            if (richTextBox1.SelectionFont.Italic)
            {
                menuFormatStyleItalic.Checked = true;
            }
            else
            {
                menuFormatStyleItalic.Checked = false;
            }
            if (richTextBox1.SelectionFont.Underline)
            {
                menuFormatStyleUnderline.Checked = true;
            }
            else
            {
                menuFormatStyleUnderline.Checked = false;
            }
            if (richTextBox1.SelectionFont.Strikeout)
            {
                menuFormatStyleStrikeOut.Checked = true;
            }
            else
            {
                menuFormatStyleStrikeOut.Checked = false;
            }
        }
        #endregion


        #region Text alignment
        /// <summary>
        /// Устанавливает выравнивание по левому краю по клику
        /// </summary>
        private void menuFormatAlignmentLeft_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
            eAlign = eAlignment.Left;
            menuAlignmentChecked();
        }


        /// <summary>
        /// Устанавливает выравнивание по правому краю по клику
        /// </summary>
        private void menuFormatAlignmentRight_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
            eAlign = eAlignment.Right;
            menuAlignmentChecked();
        }

        /// <summary>
        /// Устанавливает выравнивание по центру по клику
        /// </summary>
        private void menuFormatAlignmentCenter_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            eAlign = eAlignment.Center;
            menuAlignmentChecked();
        }

        /// <summary>
        /// Отмечает галочками пункт выравнивания в меню, который включен на данный момент
        /// </summary>
        private void menuAlignmentChecked()
        {
            switch (eAlign)
            {
                case eAlignment.Left:
                    menuFormatAlignmentLeft.Checked = true;
                    menuFormatAlignmentRight.Checked = false;
                    menuFormatAlignmentCenter.Checked = false;
                    break;
                case eAlignment.Right:
                    menuFormatAlignmentLeft.Checked = false;
                    menuFormatAlignmentRight.Checked = true;
                    menuFormatAlignmentCenter.Checked = false;
                    break;
                case eAlignment.Center:
                    menuFormatAlignmentLeft.Checked = false;
                    menuFormatAlignmentRight.Checked = false;
                    menuFormatAlignmentCenter.Checked = true;
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void sStripLayout(object sender, LayoutEventArgs e)
        {

        }

        private void menuSelect(object sender, EventArgs e)
        {
            string sSelectedString;
            switch((sender as ToolStripMenuItem).Text)
            {
                //Группа пункта меню File
                case "&File": sSelectedString = "Меню действий с файлом"; break;
                case "&New": sSelectedString = "Создать документ"; break;
                case "&Open...": sSelectedString = "Открыть новый документ"; break;
                case "&Save": sSelectedString = "Сохранить документ"; break;
                case "Save &as...": sSelectedString = "Сохранить документ как..."; break;
                case "Page Setup": sSelectedString = "Настройки страницы"; break;
                case "Print Preview": sSelectedString = "Перпросмотр печати"; break;
                case "&Print": sSelectedString = "Печать файла"; break;
                case "E&xit": sSelectedString = "Выход из программы"; break;
                //Группа пункта меню Edit
                case "&Edit": sSelectedString = "Меню редактирования"; break;
                case "&Undo": sSelectedString = "Отменить последнее действие"; break;
                case "&Redo": sSelectedString = "Возвратить последнее действие"; break;
                case "Cut": sSelectedString = "Вырезать"; break;
                case "Copy": sSelectedString = "Копировать"; break;
                case "Paste": sSelectedString = "Вставить"; break;
                case "Delete": sSelectedString = "Удалить"; break;
                case "Select All": sSelectedString = "Выделить все в поле ввода"; break;
                //Группа пункта меню Format
                case "&Format": sSelectedString = "Меню форматирования текста"; break;
                case "&Font": sSelectedString = "Настройки шрифта"; break;
                case "&Color": sSelectedString = "Цвет текста"; break;
                case "&Style": sSelectedString = "Подменю настройки стиля"; break;
                case "&Bold": sSelectedString = "Полужирный"; break;
                case "&Italic": sSelectedString = "Курсив"; break;
                case "&Underline": sSelectedString = "Подчеркнутый"; break;
                case "&Strike Out": sSelectedString = "Перечеркнутый"; break;
                case "&Alignment": sSelectedString = "Подменю настройки выравнивания"; break;
                case "&Left": sSelectedString = "Выровнять по левому краю"; break;
                case "&Right": sSelectedString = "Выровнять по правому краю"; break;
                case "&Center": sSelectedString = "Выровнять по центру"; break;
                //Группа пункта меню Help
                case "Help": sSelectedString = "Меню информации"; break;
                case "About": sSelectedString = "Информация о программе"; break;
                default:
                    sSelectedString = "";
                    break;
            }

            promptBar.Text = sSelectedString;
        }
    }
}
