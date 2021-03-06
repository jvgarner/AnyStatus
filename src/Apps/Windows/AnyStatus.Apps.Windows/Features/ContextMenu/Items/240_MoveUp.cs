﻿using AnyStatus.API.Widgets;
using AnyStatus.Apps.Windows.Features.Widgets;
using AnyStatus.Apps.Windows.Infrastructure.Mvvm;
using AnyStatus.Core.ContextMenu;

namespace AnyStatus.Apps.Windows.Features.ContextMenu.Items
{
    public class MoveUp<T> : ContextMenu<T> where T : IWidget, IMovable
    {
        public MoveUp()
        {
            Order = 240;
            Name = "Move Up";
            InputGestureText = "Alt+Up";
            Command = new Command(_ => Context.MoveUp(), _ => Context.CanMoveUp());
        }
    }
}
