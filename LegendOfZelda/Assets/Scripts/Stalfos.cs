using UnityEngine;
using System.Collections;

public class Stalfos : Enemy {
    public Sprite[] walk;

	// Use this for initialization
	void Start () {
        GoToMiddleOfTile();
        state_machine = new StateMachine();
	}
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
	}
}

public class StateStalfosNormal : State {
    Stalfos s;
    SpriteRenderer renderer;
    Sprite[] animation;

    Vector3 direction;

    public StateStalfosNormal(Stalfos _s, SpriteRenderer _renderer, Sprite[] _animation) {
        s = _s;
        renderer = _renderer;
        animation = _animation;

        bool bMoveVertical = (Random.value < 0.5);
        if(bMoveVertical) {
            direction = new Vector3(0, 1, 0);
        }
        else {
            direction = new Vector3(1, 0, 0);
        }
        bool bFlipDirection = (Random.value < 0.5);
        if(bFlipDirection) {
            direction *= -1;
        }
    }
}
