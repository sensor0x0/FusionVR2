using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.VR;
using Fusion.VR.Cosmetics;

using TMPro;

namespace Fusion.VR.Player
{
    public class FusionVRPlayer : NetworkBehaviour
    {
        public static FusionVRPlayer localPlayer;

        public int PlayerId { get; private set; }

        [Header("Objects")]
        public Transform Head;
        public Transform Body;
        public Transform LeftHand;
        public Transform RightHand;

        [Header("Colour Objects")]
        public List<Renderer> renderers = new List<Renderer>();

        [Header("Networked Transforms")]
        public NetworkTransform HeadTransform;
        public NetworkTransform LeftHandTransform;
        public NetworkTransform RightHandTransform;

        [Header("Cosmetics")]
        public List<PlayerCosmeticSlot> cosmeticSlots = new List<PlayerCosmeticSlot>();

        [Header("Other")]
        public TextMeshPro NameText;
        public bool HideLocalName = true;
        public bool HideLocalPlayer = false;

        [Header("Networked Variables")]
        public bool isLocalPlayer;  

        [Networked] public NetworkString<_32> NickName { get; set; } 
        [Networked] public Color Colour { get; set; }
        [Networked, Capacity(10)] public NetworkDictionary<NetworkString<_16>, NetworkString<_32>> Cosmetics { get; }

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (Object.HasInputAuthority)
            {
                localPlayer = this;
                isLocalPlayer = true;
                FusionVRManager.LoadPlayer();

                NameText.gameObject.SetActive(!HideLocalName);

                Head.gameObject.SetActive(!HideLocalPlayer);
                Body.gameObject.SetActive(!HideLocalPlayer);
                LeftHand.gameObject.SetActive(!HideLocalPlayer);
                RightHand.gameObject.SetActive(!HideLocalPlayer);
            }
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(NickName):
                        NameText.text = NickName.Value;
                        gameObject.name = $"Player ({NickName.Value})";
                        break;
                    case nameof(Colour):
                        foreach (Renderer renderer in renderers)
                        {
                            renderer.material.color = Colour;
                        }
                        break;
                    case nameof(Cosmetics):
                        foreach (KeyValuePair<NetworkString<_16>, NetworkString<_32>> cosmetic in Cosmetics)
                        {
                            foreach (PlayerCosmeticSlot slot in cosmeticSlots)
                            {
                                if (cosmetic.Key == slot.SlotName)
                                {
                                    foreach (Transform t in slot.Slot)
                                    {
                                        GameObject obj = t.gameObject;
                                        obj.SetActive(obj.name == cosmetic.Value);

                                        if (t.GetComponentInChildren<Collider>() != null)
                                        {
                                            Debug.LogWarning($"It is not recommended to have a collider on a cosmetic ({obj.name})");
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void Update()
        {
            if (Object.HasInputAuthority)
            {
                HeadTransform.transform.position = FusionVRManager.Manager.Head.position;
                HeadTransform.transform.rotation = FusionVRManager.Manager.Head.rotation;
                
                LeftHandTransform.transform.position = FusionVRManager.Manager.LeftHand.position;
                LeftHandTransform.transform.rotation = FusionVRManager.Manager.LeftHand.rotation;
                
                RightHandTransform.transform.position = FusionVRManager.Manager.RightHand.position;
                RightHandTransform.transform.rotation = FusionVRManager.Manager.RightHand.rotation;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasInputAuthority)
            {
                if (GetInput(out FusionVRNetworkedPlayerData data))
                {
                    HeadTransform.Teleport(data.headPosition, data.headRotation);
                    LeftHandTransform.Teleport(data.leftHandPosition, data.leftHandRotation);
                    RightHandTransform.Teleport(data.rightHandPosition, data.rightHandRotation);
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPCSetNickName(string name, RpcInfo info = default)
        {
            NickName = name;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPCSetColour(Color colour, RpcInfo info = default)
        {
            Colour = colour;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPCSetCosmetics(CosmeticSlot[] cosmetics, RpcInfo info = default)
        {
            int i = 0;
            foreach (CosmeticSlot cos in cosmetics)
            {
                if (i < Cosmetics.Capacity)
                {
                    Cosmetics.Set(cos.SlotName, cos.CosmeticName);
                }
                i++;
            }
        }
    }
}