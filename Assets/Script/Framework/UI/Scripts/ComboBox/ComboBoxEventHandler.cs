using System;

namespace Framework
{
    /// <summary>
    /// 事下拉列表件绑定
    /// </summary>
    public static class ComboBoxEventHandler
    {
        /// <summary>
        /// 绑定选择事件-绑定下拉框条目
        /// </summary>
        public static void addHandler(ComboBoxItem item, Action act)
        {
            item.OnSelect += act;
        }

        /// <summary>
        /// 绑定数值改变事件
        /// </summary>
        public static void addHandler(ComboBox comb, Action<int> act)
        {
            comb.OnSelectionChanged += act;
        } 

    }
}
