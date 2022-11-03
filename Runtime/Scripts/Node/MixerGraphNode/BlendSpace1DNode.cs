﻿using System;
using System.Collections.Generic;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Playables;

namespace GBG.AnimationGraph.Node
{
    [Serializable]
    public class BlendSpace1DSample
    {
        public AnimationClip Clip
        {
            get => _clip;
            internal set => _clip = value;
        }

        [SerializeField]
        private AnimationClip _clip;


        public float Position
        {
            get => _position;
            internal set => _position = value;
        }

        [SerializeField]
        private float _position;


        public float Speed
        {
            get => _speed;
            internal set => _speed = value;
        }

        [SerializeField]
        private float _speed = 1.0f;
    }

    [Serializable]
    public class BlendSpace1DNode : AnimationAssetPlayerNodeBase
    {
        #region Serialization Data

        public ParamGuidOrValue PositionParam
        {
            get => _positionParam;
            internal set => _positionParam = value;
        }

        [SerializeField]
        private ParamGuidOrValue _positionParam = new ParamGuidOrValue(null, 0);

        public List<BlendSpace1DSample> Samples
        {
            get => _samples;
            internal set => _samples = value;
        }

        [SerializeField]
        private List<BlendSpace1DSample> _samples = new List<BlendSpace1DSample>();

        #endregion


        #region Runtime Properties

        public override float BaseSpeed { get; protected set; }

        public override FrameData FrameData { get; protected set; }


        private ParamInfo _runtimeSpeedParam;

        private bool _runtimeSpeedDirty;

        private ParamInfo _runtimeMotionTimeParam;

        private bool _runtimeMotionTimeDirty;

        #endregion


        public BlendSpace1DNode(string guid) : base(guid)
        {
        }

        protected internal override IReadOnlyList<string> GetInputNodeGuids() => EmptyInputs;

        // TODO: GetUnscaledAnimationLength
        public override double GetUnscaledAnimationLength()
        {
            throw new NotImplementedException();
        }

        // TODO: PrepareFrame
        protected internal override void PrepareFrame(FrameData frameData) => throw new NotImplementedException();


        // TODO: InitializeParams
        protected override void InitializeParams(IReadOnlyDictionary<string, ParamInfo> paramGuidTable) =>
            throw new NotImplementedException();

        // TODO: CreatePlayable
        protected override Playable CreatePlayable(PlayableGraph playableGraph) => throw new NotImplementedException();

        // TODO: GetInputWeight
        protected override float GetLogicInputWeight(int inputIndex) => throw new NotImplementedException();
    }
}
