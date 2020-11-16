using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum PlayerType
{
    human, ai
}
[System.Serializable]
public class Player
{
    public PlayerType type= PlayerType.ai;
    public int playerNum;
    public SlotDef handSlotDef;
    public List<CardBartok> hand;

    public CardBartok AddCArd(CardBartok eCB)
    {
        if(hand==null)hand = new List<CardBartok>();
        hand.Add(eCB);
        if(type == PlayerType.human)
        {
            CardBartok[] cards = hand.ToArray();
            cards = cards.OrderBy(cd=> cd.suit).ToArray();
            hand = new List<CardBartok>(cards);
        }
        eCB.SetSortingLayerName("10");
        eCB.eventualSortLayer=handSlotDef.layerName;
        FanHand();
        return(eCB);


    }
    public CardBartok RemoveCard(CardBartok cb)
    {
        if(hand == null ||!hand.Contains(cb))return null;
        hand.Remove(cb);
        FanHand();
        return(cb);
    }
    public void FanHand()
    {
        float startRot =0;
        startRot = handSlotDef.rot;
        if(hand.Count>1)
        {
            startRot += Bartok.S.handFanDegrees*(hand.Count-1)/2;

        }
        Vector3 pos;
        float rot;
        Quaternion rotQ;
        for(int i =0; i<hand.Count; i++)
        {
            rot= startRot- Bartok.S.handFanDegrees*i;
            rotQ = Quaternion.Euler(0,0, rot);
            pos = Vector3.up*CardBartok.CARD_HEIGHT/2f;
            pos = rotQ*pos;
            pos+= handSlotDef.pos;
            pos.z=-0.5f*i;
            if(Bartok.S.phase!=TurnPhase.idle)
            {
                hand[i].timeStart=0;
            }
            hand[i].MoveTo(pos, rotQ);
            hand[i].state = CBState.toHand;
            /*
            hand[i].transform.localPosition = pos;
            hand[i].transform.rotation = rotQ;
            hand[i].state = CBState.hand;
            */
            hand[i].faceUp = (type == PlayerType.human);
            //hand[i].SetSortOrder(i*4);
            hand[i].eventualSortOrder = i*4;
            
        }
    }
    public void TakeTurn()
    {
        //cannot leave take turn, we are altering turn behavior when skippin
        //if we call pass turn in the skip turn thing, we run into issues where
        //pass turn is called on the animation's finishing. meaning we need to make sure 
        //there's a buffer.
        
        CardBartok cb;
        switch(Bartok.S.skipState)
        {
            case SkipType.too:
                Utils.tr("too was played");
                Bartok.S.phase = TurnPhase.waiting;
                cb = AddCArd(Bartok.S.Draw());
                cb.callbackPlayer = this;
                cb = AddCArd(Bartok.S.Draw());
                cb.callbackPlayer = this;

            return;

            case SkipType.none:
                //if no skip we just take turn as normal. 
                Utils.tr("none to be");
                

                if(type == PlayerType.human)return;

                Bartok.S.phase = TurnPhase.waiting;

                List<CardBartok> validCards = new List<CardBartok>();
                foreach(CardBartok tCB in hand)
                {
                    if(Bartok.S.ValidPlay(tCB))
                    {
                        validCards.Add(tCB);

                    }
                }
                if(validCards.Count==0)
                {
                    cb = AddCArd(Bartok.S.Draw());
                    cb.callbackPlayer = this;
                    return;
                }
                
                cb = validCards[Random.Range(0, validCards.Count)];
                if(cb.rank == 2)
                {
                    Bartok.S.skipState = SkipType.too;
                }
                RemoveCard(cb);
                Bartok.S.MoveToTarget(cb);

                cb.callbackPlayer = this;
                return;
            }
    }
 
    public void CBCallback(CardBartok tCB)
    {
        //oaky so this fires if a card is moved and fires when a card completes an animatioin

        //draw fires this
        //move to target fires
        //we are changing the skip state here because 
        //pass turn handles the turn taking
        //if none to be skipped, we don't skip
        //if we draw, and it too, we set it to tooskipped, but don't pass
        //there is another cbcallback soon, so we will update there.
        //if tooSkipped, we pass turn and update the state back to none. 
        
        Utils.tr("Player.CBCallback()",Bartok.S.skipState, playerNum);
        
        
        if(Bartok.S.skipState == SkipType.tooSkipped)
        {
            Bartok.S.skipState = SkipType.none;
            Utils.tr("hello11111");

        }
        
        if(Bartok.S.skipState == SkipType.none)
        {
            Bartok.S.PassTurn();
            Utils.tr("hello2222222");
        }
        if(Bartok.S.skipState==SkipType.too)
        {
            //we enter here from being played this has to fire

            
            Bartok.S.PassTurn();
            Bartok.S.skipState = SkipType.tooSkipped;
            Utils.tr("hello33333333");
        }

    } 
}
