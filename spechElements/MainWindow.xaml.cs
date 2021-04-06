using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Automation;
using System.Diagnostics;
using System.Windows.Automation.Text;
using System.Threading;
using System.Speech.Synthesis;

namespace spechElements
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechSynthesizer speechSynth;
        String last_focus;

        public MainWindow()
        {
            InitializeComponent();
            
            // создаем отслеживатель событий изменения автофокуса 

            AutomationFocusChangedEventHandler focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);



            // создаем озвучиватель текста и устанавливаем громкость
            speechSynth = new SpeechSynthesizer(); 

            speechSynth.Volume = 100;
            speechSynth.Rate = 2;

        }

        /// <summary>
        /// метод происходящий при изменения фокуса
        /// </summary>
        /// <param name="src"></param>
        /// <param name="e"></param>
        async private void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            // поместил в try..catch потому что процесса с именем notepad может не существовать.
            try
            {
                AutomationElement element = src as AutomationElement;
                String text_focus = element.Current.Name;

                

                // проверяем что бы элемент вызвавший событие изменения фокуса был из окна notepad
                if (element.Current.ProcessId == Process.GetProcessesByName("notepad")[0].Id && text_focus != last_focus)
                {
                    last_focus = text_focus;

                    speechSynth.SpeakAsyncCancelAll(); // останавливаем озвучку прошлых элементов.
                    
                    // добавляем название выбранного элемента в лог, озвучиваем и проматываем scroll вниз.
                    await this.Dispatcher.Invoke(async () =>
                    {
                        log.Text += text_focus + '\n';
                        await Task.Run(() => speechSynth.SpeakAsync(text_focus));
                        log.ScrollToEnd();                      
                    });
                }
            }
            catch (Exception)
            {

            }
           
            

        }
    }
}
