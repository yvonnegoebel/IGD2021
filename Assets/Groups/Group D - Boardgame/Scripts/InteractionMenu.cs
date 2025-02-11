﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionMenu : MonoBehaviour
{
    public PlayerAction[] actions;

    public GoldenBrickManager brickManager;

    public float bobbleSpeed = 5f;
    public float boobleAmplitude = 0.5f;

    public int goldenBrickPrice = 5;

    public Vector3 spriteOffset = new Vector3(1.2f, 0.0f, 0.0f); 

    private List<PlayerAction> possibleActions = new List<PlayerAction>();
    private int highlighted = 0;
    private PlayerData activePlayer;
    private Vector3 neverSeen = new Vector3(0f, -2000f, 0f);
    private bool takeAction = false;

    // Update is called once per frame
    void Update()
    {
        updatePossibleActions();

        renderActiveSprites();

        if (takeAction) {
            executeAction(possibleActions[highlighted]);
            takeAction = false;
        }
    }

    public void setActivePlayer(PlayerData data) {
        highlighted = 0;
        activePlayer = data;
    }

    public void nextAction() {
        highlighted = (highlighted+1) % possibleActions.Count;
    }

    public void previousAction() {
        highlighted--;
        if (highlighted < 0) {
            highlighted = possibleActions.Count - 1;
        }
    }

    public void chooseAction() {
        takeAction = activePlayer.isIdle();
    }

    private void updatePossibleActions() {
        possibleActions.Clear();
        if (activePlayer && activePlayer.isIdle()) {
            for (int i = 0; i < actions.Length; i++) {
                if (isActive(i)) {
                    possibleActions.Add(actions[i]);
                }
            }
        }
        if (highlighted >= possibleActions.Count) {
            highlighted = possibleActions.Count - 1;
        } else if (highlighted < 0) {
            highlighted = 0;
        }
    }

    private bool isActive(int i) {
        if (actions[i].requiredAP > activePlayer.actionPointsLeft()) {
            return false;
        }

        switch (actions[i].type) {
            case PlayerAction.Type.END_TURN:
                return true;
            case PlayerAction.Type.BUY_GOLDEN_BRICK:
                return activePlayer.currentTile().hasGoldenBrick() && activePlayer.creditAmount() >= goldenBrickPrice;
        }
        return false;
    }

    private void renderActiveSprites() {
        Vector3 nextPos = transform.position;

        for (int i = 0; i < actions.Length; i++) {
            if (isActive(i)) {
                // render the sprite
                Vector3 spritePos = nextPos;

                float offset = 0f;
                if (possibleActions.Count > 0 && actions[i] == possibleActions[highlighted]) {
                    // "highlighted" sprite bobbles
                    spritePos.y += Mathf.Sin(Time.timeSinceLevelLoad * bobbleSpeed) * boobleAmplitude;

                    // TODO: show AP cost and credit price (if present) of this action in the HUD
                }

                actions[i].setPosition(spritePos);
                nextPos += spriteOffset;
            }
            else {
                // prevent this from being rendered
                actions[i].setPosition(neverSeen);
            }
        }
    }

    private void executeAction(PlayerAction action) {
        switch (action.type) {
            case PlayerAction.Type.END_TURN:
                activePlayer.setActionPoints(0);
                break;
            case PlayerAction.Type.BUY_GOLDEN_BRICK:
                activePlayer.addCreditAmount(-goldenBrickPrice);
                activePlayer.addGoldenBrick();
                brickManager.relocate();
                break;
        }

        activePlayer.addActionPoints(-action.requiredAP);
    }
}
