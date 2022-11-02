﻿using System;
using System.Collections.Generic;
using System.Linq;
using GBG.AnimationGraph.Graph;
using GBG.AnimationGraph.Node;
using GBG.AnimationGraph.Parameter;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace GBG.AnimationGraph
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public partial class AnimationGraphBrain : MonoBehaviour
    {
        [SerializeField]
        private AnimationGraphAsset _graphAsset;

        private Animator _animator;

        private PlayableGraph _playableGraph;

        private NodeBase _rootNode;


        #region Mono Messages

        private void OnEnable()
        {
            if (!_playableGraph.IsValid())
            {
                _animator = GetComponent<Animator>();
                _playableGraph = PlayableGraph.Create($"{name}.{nameof(AnimationGraphBrain)}");

                // Parameters
                var paramGuidTable = new Dictionary<string, ParamInfo>(_graphAsset.Parameters.Count);
                foreach (var paramInfo in _graphAsset.Parameters)
                {
                    _paramNameTable.Add(paramInfo.Name, paramInfo);
                    paramGuidTable.Add(paramInfo.Guid, paramInfo);
                }

                // State driver
                var driverPlayable = ScriptPlayable<AnimationGraphStateDriver>.Create(_playableGraph);
                driverPlayable.GetBehaviour().Initialize(deltaTime => _rootNode?.PrepareFrame(deltaTime));
                var scriptOutput = ScriptPlayableOutput.Create(_playableGraph, "Script Output");
                scriptOutput.SetSourcePlayable(driverPlayable);

                // Animation
                BuildAnimationPlayableGraph(paramGuidTable);
            }

            _playableGraph.Play();
        }

        private void OnDisable()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Stop();
            }
        }

        private void OnDestroy()
        {
            if (_playableGraph.IsValid())
            {
                _playableGraph.Destroy();
            }
        }

        #endregion


        private void BuildAnimationPlayableGraph(Dictionary<string, ParamInfo> paramGuidTable)
        {
            var animOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation Output", _animator);
            if (!_graphAsset)
            {
                return;
            }

            // Graph layer
            var graphGuidTable = new Dictionary<string, GraphLayer>(_graphAsset.GraphLayers.Count);
            foreach (var graphLayer in _graphAsset.GraphLayers)
            {
                graphGuidTable.Add(graphLayer.Guid, graphLayer);
                graphLayer.InitializeNodes(_playableGraph, paramGuidTable);
            }

            foreach (var graphLayer in _graphAsset.GraphLayers)
            {
                graphLayer.InitializeConnections(graphGuidTable);
            }

            // Root node
            var rootGraph = _graphAsset.GraphLayers.FirstOrDefault(g => g.Guid.Equals(_graphAsset.RootGraphGuid));
            _rootNode = rootGraph?.RootNode;
            if (_rootNode == null)
            {
                return;
            }

            animOutput.SetSourcePlayable(_rootNode.Playable);
        }


        class AnimationGraphStateDriver : PlayableBehaviour
        {
            private Action<float> _onPrepareFrame;


            public void Initialize(Action<float> onPrepareFrame)
            {
                _onPrepareFrame = onPrepareFrame;
            }

            public override void PrepareFrame(Playable playable, FrameData info)
            {
                base.PrepareFrame(playable, info);

                _onPrepareFrame(info.deltaTime);
            }
        }
    }
}

/*************************************************************************************************************
 * Unity always processes **ScriptPlayableOutput** trees after `Update` message.
 * 
 * When `Animator.updateMode==AnimatorUpdateMode.AnimatePhysics` , 
 * Unity processes **AnimationPlayableOutput** tree after `FixedUpdate` message and before `Update` message, 
 * otherwise Unity processes **AnimationPlayableOutput** tree after all **ScriptPlayableOutput** trees.
 *
 * Nodes in same tree will be processed in the same order as they were added.
 * If a ScriptPlayable node is NOT in **ScriptPlayableOutput** hierarchy,
 * its **ProcessFrame** method won't be called.
 * 
 * ***********************************************************************************************************
 * 
 * Tree nodes traversal order:
 * 
 * **[On Create]** Invoked by preorder traversal:
 *  - ForEach: OnPlayableCreate
 *  - ForEach: OnGraphStart -> OnBehaviourPlay
 * 
 * **[Every Frame]** Invoked by preorder traversal:
 *  - ForEach: PrepareFrame
 * 
 * **[Every Frame]** Invoked by postorder traversal
 *  - ForEach: ProcessFrame
 *  - ForEach: ProcessRootMotion
 *  - ForEach: ProcessAnimation
 * 
 * **[On Destroy]** Invoked by preorder traversal:
 *  - ForEach: OnBehaviourPause
 *  - ForEach: OnGraphStop -> OnPlayableDestroy
 ************************************************************************************************************/