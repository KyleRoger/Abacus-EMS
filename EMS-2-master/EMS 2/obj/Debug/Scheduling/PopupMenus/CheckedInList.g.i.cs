﻿#pragma checksum "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "BD37A08239658C030B0FD96541811ED6EE8A1CDD"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using EMS_2.Scheduling.PopupMenus;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace EMS_2.Scheduling.PopupMenus {
    
    
    /// <summary>
    /// CheckedInList
    /// </summary>
    public partial class CheckedInList : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 16 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstAppointments;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnReset;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCheckIn;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnClear;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/EMS 2;component/scheduling/popupmenus/checkedinlist.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.lstAppointments = ((System.Windows.Controls.ListBox)(target));
            
            #line 16 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
            this.lstAppointments.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.LstAppointments_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnReset = ((System.Windows.Controls.Button)(target));
            
            #line 36 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
            this.btnReset.Click += new System.Windows.RoutedEventHandler(this.BtnReset_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnCheckIn = ((System.Windows.Controls.Button)(target));
            
            #line 41 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
            this.btnCheckIn.Click += new System.Windows.RoutedEventHandler(this.BtnCheckIn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnClear = ((System.Windows.Controls.Button)(target));
            
            #line 42 "..\..\..\..\Scheduling\PopupMenus\CheckedInList.xaml"
            this.btnClear.Click += new System.Windows.RoutedEventHandler(this.BtnClear_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

