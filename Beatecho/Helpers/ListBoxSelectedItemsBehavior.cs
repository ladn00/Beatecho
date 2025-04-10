﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using ListBox = System.Windows.Controls.ListBox;

namespace Beatecho.Helpers
{
    public static class ListBoxSelectedItemsBehavior
    {
        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached("BindableSelectedItems", typeof(IList), typeof(ListBoxSelectedItemsBehavior),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        public static void SetBindableSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(BindableSelectedItemsProperty, value);
        }

        public static IList GetBindableSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(BindableSelectedItemsProperty);
        }

        private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                listBox.SelectionChanged -= ListBox_SelectionChanged;
                listBox.SelectionChanged += ListBox_SelectionChanged;
            }
        }

        private static void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                var selectedItems = GetBindableSelectedItems(listBox);

                if (selectedItems == null)
                    return;

                foreach (var item in e.RemovedItems)
                {
                    if (selectedItems.Contains(item))
                        selectedItems.Remove(item);
                }

                foreach (var item in e.AddedItems)
                {
                    if (!selectedItems.Contains(item))
                        selectedItems.Add(item);
                }
            }
        }
    }

}
