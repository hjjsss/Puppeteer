﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class BlendSpace2DSample
    {
        public AnimationClip Clip
        {
            get => _clip;
            internal set => _clip = value;
        }

        [SerializeField]
        private AnimationClip _clip;


        public Vector2 Position
        {
            get => _position;
            internal set => _position = value;
        }

        [SerializeField]
        private Vector2 _position;


        public float Speed
        {
            get => _speed;
            internal set => _speed = value;
        }

        [SerializeField]
        private float _speed = 1.0f;

        // [SerializeField]
        // public bool Mirror;
    }

    [Serializable]
    public class BlendSpace2DNode : NodeBase
    {
        #region Serialization Data

        // public ParamGuidOrValue SpeedParam;

        public ParamGuidOrValue PositionXParam
        {
            get => _positionXParam;
            internal set => _positionXParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionXParam = new ParamGuidOrValue(null, 0);

        public ParamGuidOrValue PositionYParam
        {
            get => _positionYParam;
            internal set => _positionYParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionYParam = new ParamGuidOrValue(null, 0);

        public List<BlendSpace2DSample> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace2DSample> _samples = new List<BlendSpace2DSample>();

        public int[] Triangles
        {
            get => _triangles;
            internal set => _triangles = value;
        }

        [SerializeField]
        private int[] _triangles = Array.Empty<int>();

        #endregion


        public BlendSpace2DNode(string guid) : base(guid)
        {
        }

        protected internal override IList<string> GetInputNodeGuids()
        {
            return EmptyInputs;
        }
        
        protected internal override void PrepareFrame(float deltaTime)=> throw new NotImplementedException();

        
        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable)=> throw new NotImplementedException();

        protected override Playable CreatePlayable(PlayableGraph playableGraph)=> throw new NotImplementedException();

        protected override float GetInputWeight(int inputIndex)=> throw new NotImplementedException();
    }
}