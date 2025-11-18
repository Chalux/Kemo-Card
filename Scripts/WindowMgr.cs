using System;
using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using KemoCard.Pages;

namespace KemoCard.Scripts
{
    public class WindowMgr
    {
        private readonly Dictionary<string, BaseScene> _scenes = new();
        private readonly Control _dialogRoot = StaticInstance.MainRoot.GetNode<Control>("DialogRoot");
        private readonly Timer _loadTimer = StaticInstance.MainRoot.GetNode<Timer>("LoadTimer");
        private readonly Panel _clickMask = StaticInstance.MainRoot.GetNode<Panel>("ClickMask");
        private readonly Control _popUpRoot = StaticInstance.MainRoot.GetNode<Control>("PopUpRoot");
        private readonly ColorRect _blackMask = StaticInstance.MainRoot.GetNode<ColorRect>("BlackMask");

        public void PopScene(Node packedScene)
        {
            var scene = (BaseScene)packedScene;
            Debug.WriteLine("PopScene:" + scene.Name);
            AddScene(scene);
        }

        public void AddScene(BaseScene scene, params object[] datas)
        {
            if (!_scenes.TryAdd(scene.Name, scene))
            {
                return;
            }

            Debug.WriteLine("SceneAdded:" + scene.Name);

            if (scene.ShowBlackWhenAdd)
            {
                var tween = StaticInstance.MainRoot.GetTree().CreateTween();
                _blackMask.Visible = true;
                tween.TweenProperty(_blackMask, "modulate", Colors.Black, 0.5f);
                tween.TweenCallback(Callable.From(() =>
                {
                    _popUpRoot.AddChild(scene);
                    if (scene is IEvent @event)
                    {
                        StaticInstance.EventMgr.RegisterIEvent(@event);
                    }

                    tween.TweenProperty(_blackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f);
                    tween.TweenCallback(Callable.From(() =>
                    {
                        scene.OnAdd(datas);
                        StaticInstance.CurrWindow = scene;
                    }));
                }));
            }
            else
            {
                _popUpRoot.AddChild(scene);
                if (scene is IEvent @event)
                {
                    StaticInstance.EventMgr.RegisterIEvent(@event);
                }

                scene.OnAdd(datas);
                StaticInstance.CurrWindow = scene;
            }
        }

        public void RemoveScene(BaseScene scene)
        {
            if (!_scenes.ContainsKey(scene.Name)) return;
            if (scene.ShowBlackWhenRemove)
            {
                var tween = StaticInstance.MainRoot.GetTree().CreateTween();
                _blackMask.Visible = true;
                tween.TweenProperty(_blackMask, "modulate", Colors.Black, 0.5f);
                tween.TweenCallback(Callable.From(() =>
                {
                    _scenes.Remove(scene.Name);
                    GD.Print("SceneRemoved:" + scene.Name);
                    if (_popUpRoot == scene.GetParent())
                    {
                        _popUpRoot.RemoveChild(scene);
                        if (scene is IEvent @event)
                        {
                            StaticInstance.EventMgr.UnregisterIEvent(@event);
                        }
                    }

                    scene.QueueFree();
                    tween.TweenProperty(_blackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f);
                }));
            }
            else
            {
                _scenes.Remove(scene.Name);
                GD.Print("SceneRemoved:" + scene.Name);
                if (_popUpRoot == scene.GetParent())
                {
                    _popUpRoot.RemoveChild(scene);
                    if (scene is IEvent @event)
                    {
                        StaticInstance.EventMgr.UnregisterIEvent(@event);
                    }
                }

                scene.QueueFree();
            }

            var amt = PopupCount();
            if (amt > 0)
            {
                StaticInstance.CurrWindow = _popUpRoot.GetChild(amt - 1);
                return;
            }

            amt = _dialogRoot.GetChildCount();
            if (amt > 0) StaticInstance.CurrWindow = _dialogRoot.GetChild(amt - 1);
        }

        public void ChangeScene(Node node, Action<BaseScene> afterAction = null, dynamic datas = null)
        {
            var scene = (BaseScene)node;

            //Tween tween = StaticInstance.MainRoot.GetTree().CreateTween();

            _blackMask.Visible = true;
            //tween.TweenProperty(BlackMask, "modulate", Colors.Black, 0.5f);
            //tween.TweenCallback(Callable.From(() =>
            //{
            foreach (var i in _popUpRoot.GetChildren())
            {
                RemoveScene(i as BaseScene);
            }

            foreach (var i in _dialogRoot.GetChildren())
            {
                if (i != null && i.IsInsideTree())
                {
                    _dialogRoot.RemoveChild(i);
                    //i.Free();
                    i.QueueFree();
                }
            }

            _scenes.Add(scene.Name, scene);
            _dialogRoot.AddChild(scene);
            StaticInstance.CurrWindow = scene;
            if (scene is IEvent @event)
            {
                StaticInstance.EventMgr.RegisterIEvent(@event);
            }

            scene.OnAdd(datas);
            afterAction?.Invoke(scene);
            GD.Print($"SceneChanged:{scene.Name}");
            //tween.Stop();
            //Tween tween1 = StaticInstance.MainRoot.GetTree().CreateTween();
            //tween1.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f).SetDelay(1);
            //tween1.TweenCallback(Callable.From(() =>
            //{
            _blackMask.Visible = false;
            //scene.OnAdd();
            //    tween1.Stop();
            //}));
            //}));
        }

        public void ChangeScene(string sceneName, Action<BaseScene> afterAction = null, dynamic datas = null)
        {
            if (!_scenes.TryGetValue(sceneName, out var scene)) return;

            //Tween tween = StaticInstance.MainRoot.GetTree().CreateTween();

            _blackMask.Visible = true;
            //tween.TweenProperty(BlackMask, "modulate", Colors.Black, 0.5f);
            //tween.TweenCallback(Callable.From(() =>
            //{
            foreach (var i in _popUpRoot.GetChildren())
            {
                if (i.Name != sceneName) RemoveScene(i as BaseScene);
            }

            foreach (var i in _dialogRoot.GetChildren())
            {
                if (i != null && i.IsInsideTree() && i.Name != sceneName)
                {
                    _dialogRoot.RemoveChild(i);
                    i.QueueFree();
                }
            }

            StaticInstance.CurrWindow = scene;
            if (scene is IEvent @event)
            {
                StaticInstance.EventMgr.RegisterIEvent(@event);
            }

            scene.OnAdd(datas);
            afterAction?.Invoke(scene);
            GD.Print($"SceneChanged:{scene.Name}");
            //tween.Stop();
            //Tween tween1 = StaticInstance.MainRoot.GetTree().CreateTween();
            //tween1.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f).SetDelay(1);
            //tween1.TweenCallback(Callable.From(() =>
            //{
            _blackMask.Visible = false;
            //scene.OnAdd();
            //    tween1.Stop();
            //}));
            //}));
        }

        public void RemoveTopestPopup()
        {
            if (_scenes.Count > 0 && _popUpRoot.GetChildCount() > 0)
            {
                RemoveScene(_popUpRoot.GetChild(_popUpRoot.GetChildCount() - 1) as BaseScene);
            }
        }

        public int PopupCount()
        {
            return _popUpRoot.GetChildCount();
        }

        public bool IsPopupScene(string sceneName)
        {
            return _scenes.ContainsKey(sceneName);
        }

        public BaseScene GetSceneByName(string sceneName)
        {
            return _scenes.GetValueOrDefault(sceneName);
        }

        public void RemoveAllScene()
        {
            foreach (var i in _popUpRoot.GetChildren())
            {
                if (i != null && i.IsInsideTree())
                {
                    RemoveScene(i as BaseScene);
                }
            }

            foreach (var i in _scenes.Values)
            {
                if (i != null && i.IsInsideTree())
                {
                    RemoveScene(i);
                }
            }
        }

        public void RemoveSceneByName(string sceneName)
        {
            if (_scenes.TryGetValue(sceneName, out var scene))
            {
                RemoveScene(scene);
            }
        }
    }
}