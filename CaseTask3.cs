using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MathCalculator
{
    public partial class Form1 : Form
    {
        private double result = 0;
        private string operation = "";
        private bool isOperationPerformed = false;
        private string savedResult = "";

        public Form1()
        {
            InitializeComponent();
            InitializeCalculator();
        }

        private void InitializeCalculator()
        {
            // Настройка внешнего вида
            this.Text = "Математический Калькулятор";
            this.Size = new Size(350, 500);
            this.BackColor = Color.LightGray;

            // Инициализация элементов управления
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Дисплей
            textBox_Result = new TextBox();
            textBox_Result.Location = new Point(20, 20);
            textBox_Result.Size = new Size(290, 50);
            textBox_Result.Font = new Font("Arial", 16);
            textBox_Result.TextAlign = HorizontalAlignment.Right;
            textBox_Result.ReadOnly = true;
            textBox_Result.Text = "0";
            this.Controls.Add(textBox_Result);

            // Кнопки цифр
            CreateDigitButtons();

            // Кнопки операций
            CreateOperationButtons();

            // Дополнительные кнопки
            CreateFunctionButtons();

            // Меню настроек
            CreateSettingsMenu();
        }

        private void CreateDigitButtons()
        {
            string[] digits = { "7", "8", "9", "4", "5", "6", "1", "2", "3", "0" };
            int x = 20, y = 80;

            for (int i = 0; i < digits.Length; i++)
            {
                Button btn = new Button();
                btn.Text = digits[i];
                btn.Size = new Size(60, 50);
                btn.Location = new Point(x, y);
                btn.Font = new Font("Arial", 12);
                btn.Click += DigitButton_Click;

                this.Controls.Add(btn);

                x += 70;
                if ((i + 1) % 3 == 0)
                {
                    x = 20;
                    y += 60;
                }
            }
        }

        private void CreateOperationButtons()
        {
            string[] operations = { "+", "-", "×", "÷", "=", "C" };
            int x = 230, y = 80;

            foreach (string op in operations)
            {
                Button btn = new Button();
                btn.Text = op;
                btn.Size = new Size(60, 50);
                btn.Location = new Point(x, y);
                btn.Font = new Font("Arial", 12);
                btn.BackColor = GetOperationButtonColor(op);

                if (op == "=")
                    btn.Click += EqualsButton_Click;
                else if (op == "C")
                    btn.Click += ClearButton_Click;
                else
                    btn.Click += OperationButton_Click;

                this.Controls.Add(btn);
                y += 60;
            }
        }

        private void CreateFunctionButtons()
        {
            // Кнопка сохранения
            Button saveBtn = new Button();
            saveBtn.Text = "Сохранить";
            saveBtn.Size = new Size(80, 35);
            saveBtn.Location = new Point(20, 320);
            saveBtn.Font = new Font("Arial", 10);
            saveBtn.Click += SaveButton_Click;
            this.Controls.Add(saveBtn);

            // Кнопка загрузки
            Button loadBtn = new Button();
            loadBtn.Text = "Загрузить";
            loadBtn.Size = new Size(80, 35);
            loadBtn.Location = new Point(110, 320);
            loadBtn.Font = new Font("Arial", 10);
            loadBtn.Click += LoadButton_Click;
            this.Controls.Add(loadBtn);

            // Кнопка точки
            Button decimalBtn = new Button();
            decimalBtn.Text = ".";
            decimalBtn.Size = new Size(60, 50);
            decimalBtn.Location = new Point(160, 260);
            decimalBtn.Font = new Font("Arial", 12);
            decimalBtn.Click += DecimalButton_Click;
            this.Controls.Add(decimalBtn);
        }

        private void CreateSettingsMenu()
        {
            MenuStrip menuStrip = new MenuStrip();
            
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("Настройки");
            
            // Подменю изменения темы
            ToolStripMenuItem themeMenu = new ToolStripMenuItem("Цветовая схема");
            
            ToolStripMenuItem lightTheme = new ToolStripMenuItem("Светлая");
            lightTheme.Click += (s, e) => ChangeTheme("Light");
            
            ToolStripMenuItem darkTheme = new ToolStripMenuItem("Темная");
            darkTheme.Click += (s, e) => ChangeTheme("Dark");
            
            ToolStripMenuItem blueTheme = new ToolStripMenuItem("Синяя");
            blueTheme.Click += (s, e) => ChangeTheme("Blue");

            themeMenu.DropDownItems.AddRange(new[] { lightTheme, darkTheme, blueTheme });
            
            // Подменю изменения размера шрифта
            ToolStripMenuItem fontSizeMenu = new ToolStripMenuItem("Размер шрифта");
            
            ToolStripMenuItem smallFont = new ToolStripMenuItem("Маленький");
            smallFont.Click += (s, e) => ChangeFontSize(12);
            
            ToolStripMenuItem mediumFont = new ToolStripMenuItem("Средний");
            mediumFont.Click += (s, e) => ChangeFontSize(16);
            
            ToolStripMenuItem largeFont = new ToolStripMenuItem("Большой");
            largeFont.Click += (s, e) => ChangeFontSize(20);

            fontSizeMenu.DropDownItems.AddRange(new[] { smallFont, mediumFont, largeFont });

            settingsMenu.DropDownItems.AddRange(new[] { themeMenu, fontSizeMenu });
            menuStrip.Items.Add(settingsMenu);
            
            this.Controls.Add(menuStrip);
            this.MainMenuStrip = menuStrip;
        }

        private Color GetOperationButtonColor(string operation)
        {
            return operation switch
            {
                "C" => Color.LightCoral,
                "=" => Color.LightGreen,
                _ => Color.LightBlue
            };
        }

        // Обработчики событий
        private void DigitButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            
            if (textBox_Result.Text == "0" || isOperationPerformed)
                textBox_Result.Text = "";

            textBox_Result.Text += button.Text;
            isOperationPerformed = false;
        }

        private void OperationButton_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            if (double.TryParse(textBox_Result.Text, out double number))
            {
                if (!string.IsNullOrEmpty(operation))
                    CalculateResult();

                operation = button.Text;
                result = double.Parse(textBox_Result.Text);
                isOperationPerformed = true;
            }
        }

        private void EqualsButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(operation))
            {
                CalculateResult();
                operation = "";
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            textBox_Result.Text = "0";
            result = 0;
            operation = "";
            isOperationPerformed = false;
        }

        private void DecimalButton_Click(object sender, EventArgs e)
        {
            if (!textBox_Result.Text.Contains("."))
            {
                textBox_Result.Text += ".";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            savedResult = textBox_Result.Text;
            MessageBox.Show($"Результат '{savedResult}' сохранен!", "Сохранение", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(savedResult))
            {
                textBox_Result.Text = savedResult;
                MessageBox.Show($"Результат '{savedResult}' загружен!", "Загрузка", 
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Нет сохраненных результатов!", "Ошибка", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CalculateResult()
        {
            try
            {
                double secondNumber = double.Parse(textBox_Result.Text);
                
                switch (operation)
                {
                    case "+":
                        result += secondNumber;
                        break;
                    case "-":
                        result -= secondNumber;
                        break;
                    case "×":
                        result *= secondNumber;
                        break;
                    case "÷":
                        if (secondNumber == 0)
                            throw new DivideByZeroException();
                        result /= secondNumber;
                        break;
                }

                textBox_Result.Text = result.ToString();
                isOperationPerformed = true;
            }
            catch (DivideByZeroException)
            {
                textBox_Result.Text = "Ошибка: деление на 0!";
                result = 0;
                operation = "";
            }
            catch (Exception ex)
            {
                textBox_Result.Text = "Ошибка вычисления!";
                result = 0;
                operation = "";
            }
        }

        // Дополнительные функции
        private void ChangeTheme(string theme)
        {
            switch (theme)
            {
                case "Light":
                    this.BackColor = Color.LightGray;
                    textBox_Result.BackColor = Color.White;
                    break;
                case "Dark":
                    this.BackColor = Color.DimGray;
                    textBox_Result.BackColor = Color.Black;
                    textBox_Result.ForeColor = Color.White;
                    break;
                case "Blue":
                    this.BackColor = Color.LightSteelBlue;
                    textBox_Result.BackColor = Color.AliceBlue;
                    break;
            }
        }

        private void ChangeFontSize(int size)
        {
            textBox_Result.Font = new Font("Arial", size);
        }

        // Объявление элементов управления
        private TextBox textBox_Result;
    }
}
