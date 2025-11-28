using System.Linq;
using Godot;
using KemoCard.Scripts;

namespace KemoCard.Pages
{
    public enum BagOpType
    {
        PutOn,
        PutOff,
    }

    public partial class BagScene : BaseScene, IEvent
    {
        private PlayerRole _playerData;
        [Export] private EquipObject _helmetSlot;
        [Export] private EquipObject _armorSlot;
        [Export] private EquipObject _gloveSlot;
        [Export] private EquipObject _trouserSlot;
        [Export] private EquipObject _shoeSlot;
        [Export] private EquipObject _weaponSlot1;
        [Export] private EquipObject _weaponSlot2;
        [Export] private EquipObject _otherSlot1;
        [Export] private EquipObject _otherSlot2;
        [Export] private EquipObject _otherSlot3;
        [Export] private EquipObject _otherSlot4;
        [Export] private EquipObject _otherSlot5;
        [Export] private TextEdit _gmEdit;
        [Export] private Button _addBtn;
        [Export] private Button _deleteBtn;
        [Export] public PopupMenu Popup;
        [Export] private FlowContainer _flowContainer;
        [Export] private Button _quitBtn;
        [Export] private Control _debugControl;
        private Equip _popupData;
        private uint _bagIndex;
        private BagOpType _opType = BagOpType.PutOn;

        public override void OnAdd(params object[] data)
        {
            if (data?[0] is not PlayerRole)
            {
                StaticInstance.WindowMgr.RemoveScene(this);
                return;
            }

            _playerData = (PlayerRole)data[0];

            var prefab = ResourceLoader.Load<PackedScene>("res://Pages/EquipObject.tscn");
            if (prefab != null)
            {
                for (var i = 0; i < StaticEnums.BagCount; i++)
                {
                    var node = prefab.Instantiate<EquipObject>();
                    node.Init(_playerData.EquipList[i]);
                    node.AddBagDelegate();
                    _flowContainer.AddChild(node);
                }
            }

            _addBtn.Pressed += OnAddBtnOnPressed;

            Popup.IndexPressed += OnPopupOnIndexPressed;

            _quitBtn.Pressed += OnQuitBtnOnPressed;

            _helmetSlot.AddBagDelegate(2);
            _armorSlot.AddBagDelegate(2);
            _gloveSlot.AddBagDelegate(2);
            _trouserSlot.AddBagDelegate(2);
            _shoeSlot.AddBagDelegate(2);
            _weaponSlot1.AddBagDelegate(2);
            _weaponSlot2.AddBagDelegate(2);
            _otherSlot1.AddBagDelegate(2);
            _otherSlot2.AddBagDelegate(2);
            _otherSlot3.AddBagDelegate(2);
            _otherSlot4.AddBagDelegate(2);
            _otherSlot5.AddBagDelegate(2);
            _debugControl.Visible = OS.IsDebugBuild();
            UpdateScene();
        }

        private void OnQuitBtnOnPressed()
        {
            StaticInstance.WindowMgr.RemoveScene(this);
        }

        private void OnPopupOnIndexPressed(long index)
        {
            switch (index)
            {
                case 0:
                    if (_popupData != null && _playerData != null)
                    {
                        switch (_opType)
                        {
                            case BagOpType.PutOn:
                                _playerData.PutOnEquip(_bagIndex);
                                break;
                            case BagOpType.PutOff:
                                uint slot = 0;
                                foreach (var kvp in _playerData.EquipDic.Where(kvp => kvp.Value == _popupData))
                                {
                                    slot = kvp.Key;
                                }

                                _playerData.PutOffEquip(slot);
                                break;
                            default:
                                return;
                        }
                    }

                    break;
                case 1:
                    if (_playerData != null)
                    {
                        if (_opType == BagOpType.PutOff)
                        {
                            StaticInstance.MainRoot.ShowBanner("已装备的装备无法直接丢弃");
                            return;
                        }

                        var equip = _playerData.EquipList[_bagIndex];
                        equip.Owner = null;
                        _playerData.EquipList[_bagIndex] = null;
                    }

                    break;
            }

            UpdateScene();
        }

        private void OnAddBtnOnPressed()
        {
            var eqId = _gmEdit.Text;
            if (Datas.Ins.EquipPool.ContainsKey(eqId))
            {
                var idx = _playerData.GetFirstEmptyBagIndex();
                if (idx >= 0)
                {
                    _playerData.AddEquipToBag(new Equip(eqId, _playerData));
                }
                else
                {
                    StaticInstance.MainRoot.ShowBanner("无空闲空间");
                }
            }
            else
            {
                StaticInstance.MainRoot.ShowBanner("无此id装备");
            }
        }

        public override void OnRemove()
        {
            foreach (var node in _flowContainer.GetChildren())
            {
                var obj = (EquipObject)node;
                obj.Data = null;
                obj.Dispose();
            }

            _popupData = null;
        }

        private void UpdateScene()
        {
            if (_playerData == null) return;
            for (uint i = 1; i <= 12; i++)
            {
                switch (i)
                {
                    case 1:
                        if (_playerData.EquipDic.TryGetValue(i, out var helmet)) _helmetSlot.Init(helmet);
                        else _helmetSlot.Clear();
                        break;
                    case 2:
                        if (_playerData.EquipDic.TryGetValue(i, out var armor)) _armorSlot.Init(armor);
                        else _armorSlot.Clear();
                        break;
                    case 3:
                        if (_playerData.EquipDic.TryGetValue(i, out var glove))
                            _gloveSlot.Init(glove);
                        else
                            _gloveSlot.Clear();
                        break;
                    case 4:
                        if (_playerData.EquipDic.TryGetValue(i, out var trouser))
                            _trouserSlot.Init(trouser);
                        else
                            _trouserSlot.Clear();
                        break;
                    case 5:
                        if (_playerData.EquipDic.TryGetValue(i, out var shoe))
                            _shoeSlot.Init(shoe);
                        else
                            _shoeSlot.Clear();
                        break;
                    case 6:
                        if (_playerData.EquipDic.TryGetValue(i, out var weapon1))
                            _weaponSlot1.Init(weapon1);
                        else
                            _weaponSlot1.Clear();
                        break;
                    case 7:
                        if (_playerData.EquipDic.TryGetValue(i, out var weapon2))
                            _weaponSlot2.Init(weapon2);
                        else
                            _weaponSlot2.Clear();
                        break;
                    case 8:
                        if (_playerData.EquipDic.TryGetValue(i, out var other1))
                            _otherSlot1.Init(other1);
                        else
                            _otherSlot1.Clear();
                        break;
                    case 9:
                        if (_playerData.EquipDic.TryGetValue(i, out var other2))
                            _otherSlot2.Init(other2);
                        else
                            _otherSlot2.Clear();
                        break;
                    case 10:
                        if (_playerData.EquipDic.TryGetValue(i, out var other3))
                            _otherSlot3.Init(other3);
                        else
                            _otherSlot3.Clear();
                        break;
                    case 11:
                        if (_playerData.EquipDic.TryGetValue(i, out var other4))
                            _otherSlot4.Init(other4);
                        else
                            _otherSlot4.Clear();
                        break;
                    case 12:
                        if (_playerData.EquipDic.TryGetValue(i, out var other5))
                            _otherSlot5.Init(other5);
                        else
                            _otherSlot5.Clear();
                        break;
                }
            }

            for (uint i = 0; i < StaticEnums.BagCount; i++)
            {
                _flowContainer.GetChild<EquipObject>((int)i).Init(_playerData.EquipList[i]);
            }
        }

        public void ShowPopMenu(Equip data, uint bagIndex, Vector2 position, BagOpType type)
        {
            if (data == null) return;
            Popup.Visible = true;
            Popup.Position = (Vector2I)position;
            _popupData = data;
            _bagIndex = bagIndex;
            _opType = type;
        }

        public void ReceiveEvent(string @event, params object[] datas)
        {
            if (@event == "PlayerEquipUpdated")
            {
                UpdateScene();
            }
        }
    }
}