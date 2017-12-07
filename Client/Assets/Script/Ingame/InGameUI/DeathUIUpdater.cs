﻿using System;
using System.Collections;
using System.Collections.Generic;
using MyNet;
using UnityEngine;
using UnityEngine.UI;

public class DeathUIUpdater : PacketHandle {

    [SerializeField]
    GameObject dieLabel;

    [SerializeField]
    Text remainSpawnTime;

    [SerializeField]
    GameObject[] disableObjectsWhenDied;

    [SerializeField]
    MessageUpdater msgUpdater;

    float remainTime = 0.0f;
    bool isDie = false;

    private void Update()
    {
        if (isDie)
        {
            remainTime -= Time.deltaTime;
            remainSpawnTime.text = remainTime.ToString("0.##");
        }
    }


    protected override void OnMessage(Packet packet)
    {
        switch (packet.Head) {
            case Packet.HEADER.GAME_UPDATE_PLAYER_DIE:
                {
                    Packet recv = packet.Clone() as Packet;
                    remainTime = GameDefines.PlayerRespawnTime;

                    string killer = recv.Pop_String(recv.Pop_Int32());
                    string victim = recv.Pop_String(recv.Pop_Int32());

                    if (killer == Storage.Table["CurrentNickname"] as string) {
                        msgUpdater.ShowMessage("당신이 \"" + victim + "\"를 죽였습니다.");
                    }
                    else if (victim == Storage.Table["CurrentNickname"] as string)
                    {
                        dieLabel.gameObject.SetActive(true);
                        remainSpawnTime.text = remainTime.ToString("0.##");
                        isDie = true;
                        msgUpdater.ShowMessage("당신은 \"" + killer + "\"에게 죽었습니다.");
                        for (int i = 0; i < disableObjectsWhenDied.Length; i++) disableObjectsWhenDied[i].SetActive(false);
                    }
                    else {
                        remainTime = 0.0f;
                    }
                    break;
                }
            case Packet.HEADER.GAME_UPDATE_PLAYER_RESPAWN: {
                    float y = packet.Pop_Float();
                    float x = packet.Pop_Float();

                    if (packet.Pop_String(packet.Pop_Int32()) == Storage.Table["CurrentNickname"] as string) {
                        isDie = false;
                        dieLabel.gameObject.SetActive(false);
                        remainTime = 0.0f;

                        for (int i = 0; i < disableObjectsWhenDied.Length; i++) disableObjectsWhenDied[i].SetActive(true);
                    }

                    break;
                }
        }
    }
}
