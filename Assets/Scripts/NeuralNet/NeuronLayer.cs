﻿using UnityEngine;
using System.Collections;

public class NeuronLayer
{
	private Neuron[] neurons;
	private float[] outputs;
	private int totalWeights = 0;
	private int inputsCount = 0;
	private float bias;
	private	float p;

	public int NeuronsCount
	{
		get { return neurons.Length; }
	}

	public int InputsCount
	{
		get { return inputsCount; }
	}

	public int OutputsCount
	{
		get { return outputs.Length; }
	}

	public NeuronLayer(int inputsCount, int neuronsCount, float bias, float p)
	{
		this.inputsCount = inputsCount;
		this.bias = bias;
		this.p = p;

		SetNeuronsCount(neuronsCount);
	}

	void SetNeuronsCount(int neuronsCount)
	{
		neurons = new Neuron[neuronsCount];

		for (int i = 0; i < neurons.Length; i++)
		{
			neurons[i] = new Neuron(inputsCount + 1, bias, p);
			totalWeights += inputsCount + 1;
		}

		outputs = new float[neurons.Length];
	}

	public int SetWeights(float[] weights, int fromId)
	{
		for (int i = 0; i < neurons.Length; i++)
		{
			fromId = neurons[i].SetWeights(weights, fromId);
		}

		return fromId;
	}

	public float[] GetWeights()
	{
		float[] weights = new float[totalWeights];
		int id = 0;

		for (int i = 0; i < neurons.Length; i++)
		{
			float[] ws = neurons[i].GetWeights();

			for (int j = 0; j < ws.Length; j++)
			{
				weights[id] = ws[j];
				id++;
			}
		}

		return weights;
	}

	public float[] Synapsis(float[] inputs)
	{
		for (int j = 0; j < neurons.Length; j++)
		{
			outputs[j] = neurons[j].Synapsis(inputs);
		}

		return outputs;
	}
}
