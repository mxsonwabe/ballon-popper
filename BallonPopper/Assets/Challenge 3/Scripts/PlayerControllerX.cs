using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerX : MonoBehaviour
{
  public bool gameOver;

  public float floatForce;
  float topBound = 16f;
  private float gravityModifier = 1.5f;
  private Rigidbody playerRb;

  public ParticleSystem explosionParticle;
  public ParticleSystem fireworksParticle;

  private AudioSource playerAudio;
  public AudioClip moneySound;
  public AudioClip explodeSound;
  private InputAction jumpAction;

  // Start is called before the first frame update
  void Start()
  {
    Physics.gravity *= gravityModifier;
    playerAudio = GetComponent<AudioSource>();

    playerRb = GetComponent<Rigidbody>();
    // Apply a small upward force at the start of the game
    playerRb.AddForce(Vector3.up * 5, ForceMode.Impulse);

    jumpAction = InputSystem.actions.FindAction("Jump");
  }

  // Update is called once per frame
  void Update()
  {
    // While space is pressed and player is low enough, float up
    if (jumpAction.WasPerformedThisFrame() && !gameOver && transform.position.y < topBound)
    {
      Debug.Log("Jump Pressed!", this);
      playerRb.AddForce(Vector3.up * floatForce, ForceMode.Impulse);
    }
    if (transform.position.y > topBound)
    {
      Vector3 pos = transform.position;
      pos.y = topBound;
      transform.position = pos;
    }
    if (transform.position.y <= 0)
    {
      playerRb.AddForce(Vector3.up * (0.02f * floatForce), ForceMode.Impulse);
    }
  }

  private void OnCollisionEnter(Collision other)
  {
    // if player collides with bomb, explode and set gameOver to true
    if (other.gameObject.CompareTag("Bomb"))
    {
      explosionParticle.Play();
      playerAudio.PlayOneShot(explodeSound, 1.0f);
      gameOver = true;
      Debug.Log("Game Over!");
      Destroy(other.gameObject);

      //Destroy(gameObject, 2.5f);
      // wait for particle effect to end
      StartCoroutine(DestroyAfterEffect());
    }

    // if player collides with money, fireworks
    else if (other.gameObject.CompareTag("Money"))
    {
      fireworksParticle.Play();
      playerAudio.PlayOneShot(moneySound, 1.0f);
      Destroy(other.gameObject);
    }
  }

  IEnumerator DestroyAfterEffect()
  {
    // wait until all explosion particle objects are no-longer in the scene
    yield return new WaitUntil(() =>  !explosionParticle.IsAlive());
    Destroy(gameObject);
  }

}
