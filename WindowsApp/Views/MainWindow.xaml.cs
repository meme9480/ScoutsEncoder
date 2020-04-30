﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WindowsApp.Extensions;
using Core.Models.Ciphers;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Services.MorseGenerator;

namespace WindowsApp.Views
{
    public partial class MainWindow
    {
        private string CharsDelimiter => CharsDelimiterTextBox.Text;

        private string WordsDelimiter => WordsDelimiterTextBox.Text;

        private CipherBase SelectedCipher
        {
            get
            {
                var cipher = (CipherBase) CiphersComboBox.SelectedItem;
                if (cipher is MultiStandardCipher c) c.StandardIndex = StandardsComboBox.SelectedIndex;
                cipher.Key.Base = cipher.Key.IsEnabled ? KeysComboBox.SelectedIndex : 0;
                return cipher;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            NewCipherDialog.Context = this;
        }

        //// Input Event Handlers ////

        private void MirrorSelectionEventHandler(object sender, RoutedEventArgs e)
        {
            OutputRichTextBox.ClearFormatting();
            if (InputTextBox.SelectionLength == 0 || OutputRichTextBox.IsEmpty()) return;

            var encodedSelectedText   = SelectedCipher.Encode(InputTextBox.SelectedText, CharsDelimiter, WordsDelimiter);
            var precedingText         = InputTextBox.Text.Substring(0, InputTextBox.SelectionStart);
            var encodedPrecedingText  = SelectedCipher.Encode(precedingText, CharsDelimiter, WordsDelimiter);

            var outputStartPointer    = OutputRichTextBox.GetStart();
            var highlightStartPointer = outputStartPointer.GetPositionAtOffset(encodedPrecedingText.Length + 2);
            var highlightEndPointer   = highlightStartPointer?.GetPositionAtOffset(encodedSelectedText.Length + 2) ?? OutputRichTextBox.GetEnd();
            var highlightTextRange    = new TextRange(highlightStartPointer, highlightEndPointer);

            highlightTextRange.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.CornflowerBlue);
        }


        //// Output Event Handlers & Properties ////


        private void ExportAudioButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = "MorseCode - " + AudioSpeedComboBox.Text,
                DefaultExt = ".wav",
                Filter = "Waveform Audio File|*.wav"
            };

            if (saveFileDialog.ShowDialog() != true) return;

            var text      = OutputRichTextBox.GetText();
            var speed     = AudioSpeedComboBox.SelectedIndex;
            var fileName  = saveFileDialog.FileName;
            
            MorseAudioGenerator.Generate(fileName, text, CharsDelimiter, WordsDelimiter, speed);

            var content = $"{saveFileDialog.SafeFileName} is Saved!";
            var arguments = $"/select, \"{fileName}\"";
            void Action() => Process.Start("explorer.exe", arguments);
            Snackbar.MessageQueue.Enqueue(content, "Open", Action);
        }
    }
}