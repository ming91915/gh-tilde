﻿namespace TILDA
{
    using MathNet.Numerics.LinearAlgebra.Double;
    using MathNet.Numerics.LinearAlgebra.Generic;
    using StructureEngine.MachineLearning;
    using StructureEngine.MachineLearning.Testing;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal class ProblemBuilder
    {
        private TILDA.TILDAComponent component;
        private SurrogateModelBuilder model;
        private RegCase r;
        private RegressionReport rr;

        public ProblemBuilder(TILDA.TILDAComponent component)
        {
            this.component = component;
            component.model = this.model;
        }

        public double Predict(List<double> Features)
        {
            Observation test = new Observation(Features, 0.0);
            return this.rr.Model.Predict(test);
        }

        public void Start()
        {
            int num5;
            List<double> list2;
            int num6;
            Observation observation;
            MessageBox.Show("Starting...");
            this.model = new SurrogateModelBuilder();
            double ratio = this.component.ratio;
            int num2 = (int) Math.Round((double) (this.component.designMap.Count * ratio), 0);
            int num3 = this.component.designMap.Count - num2;
            List<Observation> trainSet = new List<Observation>();
            int num4 = 0;
            for (num5 = 0; num5 < num2; num5++)
            {
                list2 = new List<double>();
                num6 = 0;
                while (num6 < this.component.numVariables)
                {
                    list2.Add(this.component.designMap[num5][num6]);
                    num4++;
                    num6++;
                }
                observation = new Observation(list2, this.component.designMap[num5][this.component.numVariables]);
                trainSet.Add(observation);
            }
            List<Observation> valSet = new List<Observation>();
            for (num5 = num4; num5 < this.component.designMap.Count; num5++)
            {
                list2 = new List<double>();
                for (num6 = 0; num6 < this.component.numVariables; num6++)
                {
                    list2.Add(this.component.designMap[num5][num6]);
                }
                observation = new Observation(list2, this.component.designMap[num5][this.component.numVariables]);
                valSet.Add(observation);
            }
            Matrix<double> w = new DenseMatrix(1, 6, new double[] { 0.0, 0.0, 1.0, 1.0, 1.0, 1.0 });
            this.r = new RegCase(true, true, true, w, null);
            MessageBox.Show("Building Model: A message will appear when is finished");
            this.component.rr = this.model.BuildModel(this.r, trainSet, valSet);
            MessageBox.Show("Finished: Model built");
            this.component.modelCreated = true;

            string modelType = "";
            if (this.component.rr.Model is EnsembleNeuralNetRegression)
            {
                modelType = "Ensemble Neural Net";
            }
            else if (this.component.rr.Model is RandomForestRegression)
            {
                modelType = "Random Forest";
            }
            else if (this.component.rr.Model is KrigingRegression)
            {
                modelType = "Kriging";
            }

            string nuisanceParam = this.component.rr.Model.Parameter.ToString();

        }
    }
}

