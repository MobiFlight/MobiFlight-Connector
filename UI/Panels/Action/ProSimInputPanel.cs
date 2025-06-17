﻿using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class ProSimInputPanel : UserControl, IPanelConfigSync
    {
        public ProSimInputPanel()
        {
            InitializeComponent();
            proSimDatarefPanel1.SetMode(false);
        }

        public void syncFromConfig(object config)
        {
            if ((config is ProSimInputAction proSimInputAction))
            {
                proSimDatarefPanel1.Path = proSimInputAction.Path;
                proSimDatarefPanel1.TransformOptionsGroup.syncFromConfig(proSimInputAction);
            }
            else if (config is FsuipcOffsetInputAction fsuipcOffsetInputAction)
            {
                proSimDatarefPanel1.TransformOptionsGroup.syncFromConfig(fsuipcOffsetInputAction);
            }
            proSimDatarefPanel1.TransformOptionsGroup.ShowValuePanel(true);
        }

        public void Init(IExecutionManager executionManager)
        {
            proSimDatarefPanel1.Init(executionManager);
        }

        public InputAction ToConfig()
        {
            var action = new ProSimInputAction
            {

                Path = proSimDatarefPanel1.Path.Trim()
            };

            proSimDatarefPanel1.TransformOptionsGroup.syncToConfig(action);

            return action;
        }

        private void proSimDatarefPanel1_Load(object sender, EventArgs e)
        {
            proSimDatarefPanel1.LoadDataRefDescriptions();
        }
    }
}
