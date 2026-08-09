using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LinyFaktorialWpf
{
    public partial class MainWindow : Window
    {
        private int pocetVypoctu = 0;
        private readonly Random rng = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        // === 1. UDÁLOST: ÚTEK PŘED CURSOREM ===
        private void BtnSpocitej_MouseEnter(object sender, MouseEventArgs e)
        {
            // Tlačítko uteče s pravděpodobností 70 % (ať má uživatel aspoň malou šanci)
            if (rng.NextDouble() < 0.75)
            {
                double maxLeft = hlavniPlatno.ActualWidth - btnSpocitej.ActualWidth;
                double maxTop = hlavniPlatno.ActualHeight - btnSpocitej.ActualHeight;

                if (maxLeft <= 0 || maxTop <= 0) return;

                double noveLeft = rng.NextDouble() * maxLeft;
                double noveTop = rng.NextDouble() * maxTop;

                Canvas.SetLeft(btnSpocitej, noveLeft);
                Canvas.SetTop(btnSpocitej, noveTop);
            }
        }

        // === 2. UDÁLOST: VÝPOČET FAKTORIÁLU (Když ho uživatel chytne) ===
        private async void BtnSpocitej_Click(object sender, RoutedEventArgs e)
        {
            // Kontrola vstupu
            if (!int.TryParse(txtVstup.Text, out int cislo))
            {
                NastavVystup("Tohle asi není číslo...", Colors.Red);
                return;
            }

            // Režim línosti a únavy
            if (pocetVypoctu >= 3)
            {
                NastavVystup("Jsem unavený. Už toho bylo pro dnešek dost!", Colors.OrangeRed);
                return;
            }

            if (rng.Next(0, 5) == 0)
            {
                NastavVystup("Dneska se mi fakt nechce.", Colors.OrangeRed);
                return;
            }

            // Blokace tlačítek a spuštění asynchronní animace
            btnSpocitej.IsEnabled = false;
            NastavVystup("Auuu! Chytil jsi mě... Tak já přemýšlím.", Colors.Cyan);

            await AnimujPremysleniAsync(cislo);

            // Výpočet
            long vysledek = SpocitejFaktorial(cislo);
            pocetVypoctu++;

            NastavVystup($"No dobře... {cislo}! = {vysledek}", Colors.LimeGreen);
            btnSpocitej.IsEnabled = true;
        }

        // === POMOCNÉ METODY ===
        private async Task AnimujPremysleniAsync(int cislo)
        {
            progressPremysleni.Visibility = Visibility.Visible;
            progressPremysleni.Value = 0;

            int cykly = Math.Clamp(cislo * 5, 20, 100);

            for (int i = 0; i <= cykly; i++)
            {
                progressPremysleni.Value = (double)i / cykly * 100;
                await Task.Delay(30); // Asynchronní uvolnění vlákna
            }

            progressPremysleni.Visibility = Visibility.Hidden;
        }

        private static long SpocitejFaktorial(int n)
        {
            long vysledek = 1;
            for (int i = 1; i <= n; i++) vysledek *= i;
            return vysledek;
        }

        private void NastavVystup(string text, Color barva)
        {
            lblVystup.Text = text;
            lblVystup.Foreground = new SolidColorBrush(barva);
        }
    }
}