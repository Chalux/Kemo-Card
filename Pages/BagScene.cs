using Godot;
using KemoCard.Scripts;
using StaticClass;

namespace KemoCard.Pages
{
    public enum BagOpType
    {
        PUT_ON,
        PUT_OFF,
    }
    public partial class BagScene : BaseScene, IEvent
    {
        private PlayerRole playerData;
        [Export] EquipObject HalmetSlot;
        [Export] EquipObject ArmorSlot;
        [Export] EquipObject GloveSlot;
        [Export] EquipObject TrouserSlot;
        [Export] EquipObject ShoeSlot;
        [Export] EquipObject WeaponSlot1;
        [Export] EquipObject WeaponSlot2;
        [Export] EquipObject OtherSlot1;
        [Export] EquipObject OtherSlot2;
        [Export] EquipObject OtherSlot3;
        [Export] EquipObject OtherSlot4;
        [Export] EquipObject OtherSlot5;
        [Export] TextEdit GMEdit;
        [Export] Godot.Button AddBtn;
        [Export] Godot.Button DeleteBtn;
        [Export] public PopupMenu popup;
        [Export] FlowContainer flowContainer;
        [Export] Godot.Button QuitBtn;
        [Export] Control DebugControl;
        private Equip popupdata;
        private uint bagIndex;
        private BagOpType opType = BagOpType.PUT_ON;
        public override void OnAdd(dynamic data)
        {
            if ((data == null && data[0] != null) || data[0] is not PlayerRole)
            {
                StaticInstance.windowMgr.RemoveScene(this);
                return;
            }
            else
            {
                playerData = data[0];
            }
            var prefab = ResourceLoader.Load<PackedScene>("res://Pages/EquipObject.tscn");
            if (prefab != null)
            {
                for (int i = 0; i < StaticEnums.BagCount; i++)
                {
                    var node = prefab.Instantiate<EquipObject>();
                    node.Init(playerData.EquipList[i]);
                    node.AddBagDelegate(1);
                    flowContainer.AddChild(node);
                }
            }
            AddBtn.Pressed += new(() =>
            {
                string eqid = GMEdit.Text;
                if (Datas.Ins.EquipPool.ContainsKey(eqid))
                {
                    int idx = playerData.GetFirstEmptyBagIndex();
                    if (idx >= 0)
                    {
                        playerData.AddEquipToBag(new(eqid, playerData));
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
            });

            popup.IndexPressed += new((index) =>
            {
                switch (index)
                {
                    case 0:
                        if (popupdata != null && playerData != null)
                        {
                            switch (opType)
                            {
                                case BagOpType.PUT_ON:
                                    playerData.PutOnEquip(bagIndex);
                                    break;
                                case BagOpType.PUT_OFF:
                                    uint slot = 0;
                                    foreach (var kvp in playerData.EquipDic)
                                    {
                                        if (kvp.Value == popupdata)
                                        {
                                            slot = kvp.Key;
                                        }
                                    }
                                    playerData.PutOffEquip(slot);
                                    break;
                            }
                        }
                        break;
                    case 1:
                        if (playerData != null)
                        {
                            if (opType == BagOpType.PUT_OFF)
                            {
                                StaticInstance.MainRoot.ShowBanner("已装备的装备无法直接丢弃");
                                return;
                            }
                            var equip = playerData.EquipList[bagIndex];
                            equip.owner = null;
                            equip = null;
                            playerData.EquipList[bagIndex] = null;
                        }
                        break;
                }
                UpdateScene();
            });

            QuitBtn.Pressed += new(() =>
            {
                StaticInstance.windowMgr.RemoveScene(this);
            });

            HalmetSlot.AddBagDelegate(2);
            ArmorSlot.AddBagDelegate(2);
            GloveSlot.AddBagDelegate(2);
            TrouserSlot.AddBagDelegate(2);
            ShoeSlot.AddBagDelegate(2);
            WeaponSlot1.AddBagDelegate(2);
            WeaponSlot2.AddBagDelegate(2);
            OtherSlot1.AddBagDelegate(2);
            OtherSlot2.AddBagDelegate(2);
            OtherSlot3.AddBagDelegate(2);
            OtherSlot4.AddBagDelegate(2);
            OtherSlot5.AddBagDelegate(2);
            DebugControl.Visible = OS.IsDebugBuild();
            UpdateScene();
        }

        public override void OnRemove()
        {
            foreach (EquipObject obj in flowContainer.GetChildren())
            {
                obj.Data = null;
                obj.Dispose();
            }
            popupdata = null;
        }

        void UpdateScene()
        {
            if (playerData != null)
            {
                for (uint i = 1; i <= 12; i++)
                {
                    switch (i)
                    {
                        case 1:
                            if (playerData.EquipDic.ContainsKey(i)) HalmetSlot.Init(playerData.EquipDic[i]);
                            else HalmetSlot.Clear();
                            break;
                        case 2:
                            if (playerData.EquipDic.ContainsKey(i)) ArmorSlot.Init(playerData.EquipDic[i]);
                            else ArmorSlot.Clear();
                            break;
                        case 3:
                            if (playerData.EquipDic.ContainsKey(i))
                                GloveSlot.Init(playerData.EquipDic[i]);
                            else
                                GloveSlot.Clear(); break;
                        case 4:
                            if (playerData.EquipDic.ContainsKey(i))
                                TrouserSlot.Init(playerData.EquipDic[i]);
                            else
                                TrouserSlot.Clear(); break;
                        case 5:
                            if (playerData.EquipDic.ContainsKey(i))
                                ShoeSlot.Init(playerData.EquipDic[i]);
                            else
                                ShoeSlot.Clear(); break;
                        case 6:
                            if (playerData.EquipDic.ContainsKey(i))
                                WeaponSlot1.Init(playerData.EquipDic[i]);
                            else
                                WeaponSlot1.Clear(); break;
                        case 7:
                            if (playerData.EquipDic.ContainsKey(i))
                                WeaponSlot2.Init(playerData.EquipDic[i]);
                            else
                                WeaponSlot2.Clear(); break;
                        case 8:
                            if (playerData.EquipDic.ContainsKey(i))
                                OtherSlot1.Init(playerData.EquipDic[i]);
                            else
                                OtherSlot1.Clear(); break;
                        case 9:
                            if (playerData.EquipDic.ContainsKey(i))
                                OtherSlot2.Init(playerData.EquipDic[i]);
                            else
                                OtherSlot2.Clear(); break;
                        case 10:
                            if (playerData.EquipDic.ContainsKey(i))
                                OtherSlot3.Init(playerData.EquipDic[i]);
                            else
                                OtherSlot3.Clear(); break;
                        case 11:
                            if (playerData.EquipDic.ContainsKey(i))
                                OtherSlot4.Init(playerData.EquipDic[i]);
                            else
                                OtherSlot4.Clear(); break;
                        case 12:
                            if (playerData.EquipDic.ContainsKey(i))
                                OtherSlot5.Init(playerData.EquipDic[i]);
                            else
                                OtherSlot5.Clear(); break;
                    }
                }
                for (uint i = 0; i < StaticEnums.BagCount; i++)
                {
                    flowContainer.GetChild<EquipObject>((int)i).Init(playerData.EquipList[i]);
                }
            }
        }

        public void ShowPopMenu(Equip data, uint bagIndex, Vector2 Position, BagOpType type)
        {
            if (data != null)
            {
                popup.Visible = true;
                popup.Position = (Vector2I)Position;
                popupdata = data;
                this.bagIndex = bagIndex;
                opType = type;
            }
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