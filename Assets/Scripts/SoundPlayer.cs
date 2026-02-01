using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{

    [SerializeField] AudioClip attackEnemy;
    [SerializeField] AudioClip enemyAttack;
    [SerializeField] AudioClip cardSound;
    [SerializeField] AudioClip diceRoll;
    [SerializeField] AudioClip shopBuySound;

    public static SoundPlayer Instance { get; private set; }

    private AudioSource audioSource;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cache AudioSource once
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("MusicPlayer: No AudioSource component found!");
        }
    }


    public void PlayAttackenemySound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == attackEnemy && audioSource.isPlaying)
            return;
        audioSource.clip = attackEnemy;
        audioSource.Play();

    }

    public void PlayEnemyAttackSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == enemyAttack && audioSource.isPlaying)
            return;
        audioSource.clip = enemyAttack;
        audioSource.Play();
    }

    public void PlayCardSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == cardSound && audioSource.isPlaying)
            return;
        audioSource.clip = cardSound;
        audioSource.Play();
    }

    public void PlayDicerollSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == diceRoll && audioSource.isPlaying)
            return;
        audioSource.clip = diceRoll;
        audioSource.Play();
    }

    public void PlayShopBuySound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == shopBuySound && audioSource.isPlaying)
            return;
        audioSource.clip = shopBuySound;
        audioSource.Play();
    }
}
