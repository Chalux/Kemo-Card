using Godot;
using KemoCard.Pages;
using StaticClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace KemoCard.Scripts
{
    public class WindowMgr
    {
        Dictionary<string, BaseScene> _scenes = new();
        readonly Node2D DialogRoot;
        readonly Timer loadTimer;
        readonly Panel ClickMask;
        readonly Node2D PopUpRoot;
        readonly ColorRect BlackMask;
        public WindowMgr()
        {
            DialogRoot = StaticInstance.MainRoot.GetNode<Node2D>("DialogRoot");
            loadTimer = StaticInstance.MainRoot.GetNode<Timer>("LoadTimer");
            ClickMask = StaticInstance.MainRoot.GetNode<Panel>("ClickMask");
            PopUpRoot = StaticInstance.MainRoot.GetNode<Node2D>("PopUpRoot");
            BlackMask = StaticInstance.MainRoot.GetNode<ColorRect>("BlackMask");
        }

        public void PopScene(Node packedScene)
        {
            BaseScene scene = (BaseScene)packedScene;
            Debug.WriteLine("PopScene:" + scene.Name);
            AddScene(scene);
        }

        public void AddScene(BaseScene scene, params object[] datas)
        {
            if (_scenes.ContainsKey(scene.Name))
            {
                return;
            }
            _scenes.Add(scene.Name, scene);
            Debug.WriteLine("SceneAdded:" + scene.Name);

            if (scene.ShowBlackWhenAdd)
            {
                Tween tween = StaticInstance.MainRoot.GetTree().CreateTween();
                BlackMask.Visible = true;
                tween.TweenProperty(BlackMask, "modulate", Colors.Black, 0.5f);
                tween.TweenCallback(Callable.From(() =>
                {
                    PopUpRoot.AddChild(scene);
                    if (scene is IEvent @event)
                    {
                        StaticInstance.eventMgr.RegistIEvent(@event);
                    }
                    tween.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f);
                    tween.TweenCallback(Callable.From(() =>
                    {
                        scene.OnAdd(datas);
                        StaticInstance.currWindow = scene;
                    }));
                }));
            }
            else
            {
                PopUpRoot.AddChild(scene);
                if (scene is IEvent @event)
                {
                    StaticInstance.eventMgr.RegistIEvent(@event);
                }
                scene.OnAdd(datas);
                StaticInstance.currWindow = scene;
            }
        }

        public void RemoveScene(BaseScene scene)
        {
            if (!_scenes.ContainsKey(scene.Name)) return;
            if (scene.ShowBlackWhenRemove)
            {
                Tween tween = StaticInstance.MainRoot.GetTree().CreateTween();
                BlackMask.Visible = true;
                tween.TweenProperty(BlackMask, "modulate", Colors.Black, 0.5f);
                tween.TweenCallback(Callable.From(() =>
                {
                    _scenes.Remove(scene.Name);
                    GD.Print("SceneRemoved:" + scene.Name);
                    if (PopUpRoot == scene.GetParent())
                    {
                        PopUpRoot.RemoveChild(scene);
                        if (scene is IEvent @event)
                        {
                            StaticInstance.eventMgr.UnregistIEvent(@event);
                        }
                    }
                    scene.QueueFree();
                    tween.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f);
                }));
            }
            else
            {
                _scenes.Remove(scene.Name);
                GD.Print("SceneRemoved:" + scene.Name);
                if (PopUpRoot == scene.GetParent())
                {
                    PopUpRoot.RemoveChild(scene);
                    if (scene is IEvent @event)
                    {
                        StaticInstance.eventMgr.UnregistIEvent(@event);
                    }
                }
                scene.QueueFree();
            }
            var amt = PopupCount();
            if (amt > 0)
            {
                StaticInstance.currWindow = PopUpRoot.GetChild(amt - 1);
                return;
            }
            amt = DialogRoot.GetChildCount();
            if (amt > 0) StaticInstance.currWindow = DialogRoot.GetChild(amt - 1);
        }

        public void ChangeScene(Node node, Action<BaseScene> afterAction = null, dynamic datas = null)
        {
            BaseScene scene = (BaseScene)node;

            //Tween tween = StaticInstance.MainRoot.GetTree().CreateTween();

            BlackMask.Visible = true;
            //tween.TweenProperty(BlackMask, "modulate", Colors.Black, 0.5f);
            //tween.TweenCallback(Callable.From(() =>
            //{
            foreach (var i in PopUpRoot.GetChildren())
            {
                RemoveScene(i as BaseScene);
            }
            foreach (var i in DialogRoot.GetChildren())
            {
                if (i != null && i.IsInsideTree())
                {
                    DialogRoot.RemoveChild(i);
                    //i.Free();
                    i.QueueFree();
                }
            }
            _scenes.Add(scene.Name, scene);
            DialogRoot.AddChild(scene);
            StaticInstance.currWindow = scene;
            if (scene is IEvent @event)
            {
                StaticInstance.eventMgr.RegistIEvent(@event);
            }
            scene.OnAdd(datas);
            afterAction?.Invoke(scene);
            GD.Print($"SceneChanged:{scene.Name}");
            //tween.Stop();
            //Tween tween1 = StaticInstance.MainRoot.GetTree().CreateTween();
            //tween1.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f).SetDelay(1);
            //tween1.TweenCallback(Callable.From(() =>
            //{
            BlackMask.Visible = false;
            //scene.OnAdd();
            //    tween1.Stop();
            //}));
            //}));
        }

        public void ChangeScene(string SceneName, Action<BaseScene> afterAction = null, dynamic datas = null)
        {
            if (!_scenes.ContainsKey(SceneName)) return;
            BaseScene scene = _scenes[SceneName];

            //Tween tween = StaticInstance.MainRoot.GetTree().CreateTween();

            BlackMask.Visible = true;
            //tween.TweenProperty(BlackMask, "modulate", Colors.Black, 0.5f);
            //tween.TweenCallback(Callable.From(() =>
            //{
            foreach (var i in PopUpRoot.GetChildren())
            {
                if (i.Name != SceneName) RemoveScene(i as BaseScene);
            }
            foreach (var i in DialogRoot.GetChildren())
            {
                if (i != null && i.IsInsideTree() && i.Name != SceneName)
                {
                    DialogRoot.RemoveChild(i);
                    i.QueueFree();
                }
            }
            StaticInstance.currWindow = scene;
            if (scene is IEvent @event)
            {
                StaticInstance.eventMgr.RegistIEvent(@event);
            }
            scene.OnAdd(datas);
            afterAction?.Invoke(scene);
            GD.Print($"SceneChanged:{scene.Name}");
            //tween.Stop();
            //Tween tween1 = StaticInstance.MainRoot.GetTree().CreateTween();
            //tween1.TweenProperty(BlackMask, "modulate", Color.Color8(0, 0, 0, 0), 0.5f).SetDelay(1);
            //tween1.TweenCallback(Callable.From(() =>
            //{
            BlackMask.Visible = false;
            //scene.OnAdd();
            //    tween1.Stop();
            //}));
            //}));
        }

        public void RemoveTopestPopup()
        {
            if (_scenes.Count > 0 && PopUpRoot.GetChildCount() > 0)
            {
                RemoveScene(PopUpRoot.GetChild(PopUpRoot.GetChildCount() - 1) as BaseScene);
            }
        }

        public int PopupCount()
        {
            return PopUpRoot.GetChildCount();
        }

        public bool IsPopupScene(string sceneName)
        {
            return _scenes.ContainsKey(sceneName);
        }

        public BaseScene GetSceneByName(string sceneName)
        {
            return _scenes.ContainsKey(sceneName) ? _scenes[sceneName] : null;
        }

        public void RemoveAllScene()
        {
            foreach (var i in PopUpRoot.GetChildren())
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
    }
}
