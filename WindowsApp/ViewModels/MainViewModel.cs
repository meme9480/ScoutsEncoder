﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using WindowsApp.Extensions;
using WindowsApp.ViewModels.Common;
using Core.Data;
using Core.Models.Ciphers;
using MaterialDesignThemes.Wpf;

namespace WindowsApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public SnackbarMessageQueue MessageQueue { get; set; } = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
        public CipherViewModel Cipher { get; set; } = new CipherViewModel();
        public IEnumerable<CipherBase> Ciphers => CiphersList.Instance;
        public CommandBase<RichTextBox> Copy { get; set; } = new CommandBase<RichTextBox>(r => r.CopyToClipboard());
        public CommandBase<RichTextBox> Clear { get; set; } = new CommandBase<RichTextBox>(r => r.Clear());
    }
}
