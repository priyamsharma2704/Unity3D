using UnityEngine;
using System.Collections;

public class SoundVisual : MonoBehaviour {

    private const int SAMPLE_SIZE = 1024;

    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    public float visaulModifier = 500.0f;
    public float smoothSpeed = 10.0f;
    public float maxVisualScale = 7.0f;
    public float keepPercentage = 0.5f;

    public float bgIntensity;
    public Material bgMat;
    public Color minColor;
    public Color maxColor;
     

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;

    private Transform[] visualList;
    private float[] visualScale;
    public int amnVisual = 64;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;

        //SpawnLine(); 
        SpawnCircle();
	}

    private void SpawnLine()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        for (int i = 0; i < amnVisual; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visualList[i] = go.transform;
            visualList[i].position = Vector3.right * i;

        }


    }
    private void SpawnCircle()
    {
        visualScale = new float[amnVisual];
        visualList = new Transform[amnVisual];

        Vector3 center = Vector3.zero;
        float radius = 10.0f;
        for (int i = 0; i < amnVisual; i++)
        {
            float angle = i * 1.0f / amnVisual;
            angle = angle * Mathf.PI * 2;
            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;

            Vector3 pos =  center+ new Vector3(x,y,0);
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            go.transform.position = pos;
            go.transform.rotation = Quaternion.LookRotation(Vector3.forward, pos);
            visualList[i] = go.transform;
        }
    }

    private void UpdateVisual()
    {
        int visualIndex = 0;
        int SpectrumIndex = 0;
        int averageSize = (int)((SAMPLE_SIZE * keepPercentage) / amnVisual);

        while (visualIndex < amnVisual)
        {
            int j = 0;
            float sum = 0;
            while (j < averageSize)
            {
                sum += spectrum[SpectrumIndex];
                SpectrumIndex++;
                j++;

            }

            float scaleY = sum / averageSize * visaulModifier;
            visualScale[visualIndex] -= Time.deltaTime * smoothSpeed;
            if (visualScale[visualIndex] < scaleY)
            {
                visualScale[visualIndex] = scaleY;
            }

            if (visualScale[visualIndex] > maxVisualScale)
            {
                visualScale[visualIndex] = maxVisualScale;
            }
            visualList[visualIndex].localScale = Vector3.one + Vector3.up * visualScale[visualIndex];
            visualIndex++;

        }
    }
	// Update is called once per frame
	void Update () {
        AnalyzeSound(); //analyse the sound for every single frame 
        UpdateVisual();

        UpdateBg();
	
	}
    private void UpdateBg()
    {
        bgIntensity -= Time.deltaTime * 0.5f;
        if (bgIntensity < dbValue/40)
            bgIntensity = dbValue/40 ;
        bgMat.color = Color.Lerp(maxColor, minColor, -bgIntensity);

    }
    private void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);   //signal i sbeign passed as reference
        
        //get rms value
        int i = 0;
        float sum = 0;
        for (; i < SAMPLE_SIZE; i++)
        {
            sum = samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        //get db value
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        //get sound spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        //Find pitch value



    }
}
