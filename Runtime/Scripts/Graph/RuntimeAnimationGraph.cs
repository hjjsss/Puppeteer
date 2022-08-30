﻿using System;
using System.Collections.Generic;
using GBG.Puppeteer.NodeData;
using GBG.Puppeteer.Parameter;
using UnityEngine;

namespace GBG.Puppeteer.Graph
{
    [CreateAssetMenu(fileName = nameof(RuntimeAnimationGraph),
        menuName = "Puppeteer/[TEST] Runtime Animation Graph")]
    public class RuntimeAnimationGraph : ScriptableObject
    {
        public IReadOnlyList<ParamInfo> Parameters => _parameters;

        [SerializeField]
        private ParamInfo[] _parameters = Array.Empty<ParamInfo>();


        // Node at index 0 is always the output node
        public IReadOnlyList<AnimationNodeData> Nodes => _nodes;

        [SerializeReference]
        private AnimationNodeData[] _nodes = Array.Empty<AnimationNodeData>();


        #region EDITOR DATA

#if UNITY_EDITOR
        internal AnimationNodeData[] EditorIsolatedNodes
        {
            get => _editorIsolatedNodes;
            set => _editorIsolatedNodes = value;
        }

        [SerializeReference]
        private AnimationNodeData[] _editorIsolatedNodes = Array.Empty<AnimationNodeData>();


        internal ParamInfo[] EditorParameters
        {
            get => _parameters;
            set => _parameters = value;
        }

        internal AnimationNodeData[] EditorNodes
        {
            get => _nodes;
            set => _nodes = value;
        }
#endif

        #endregion


        // FOR TEST
        private void Reset()
        {
            _nodes = new AnimationNodeData[]
            {
                new AnimationClipNodeData(),
                // new AnimationMixerNodeData(),
                // new AnimationLayerMixerNodeData(),
                // new AnimationScriptNodeData(),
                // new AnimationSubGraphNodeData(),
            };
        }
    }
}