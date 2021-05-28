using System;
namespace BI_Project.Models.UI
{
    public class BlockModel
    {
        public object DataModel { set; get; }

        public BI_Project.Models.UI.BlockLanguageModel LanguageModel { set; get; }

        

        public string BlockName { set; get; }

        public string BlockId { set; get; }

        public int Hidden { set; get; }

        public BlockModel()
        {

        }

        public BlockModel(string blockName)
        {
            this.BlockName = blockName;
            this.LanguageModel = new BlockLanguageModel();
            this.LanguageModel.BlockName = blockName;
            this.BlockId = this.BlockName + "_1";
        }

        public BlockModel(string blockName,Object languageObjet)
        {
            this.BlockName = blockName;
            this.LanguageModel = new BlockLanguageModel();
            this.LanguageModel.BlockName = blockName;
            this.BlockId = this.BlockName + "_1";

            this.LanguageModel.SetLanguage(languageObjet);
        }
        public BlockModel(string blockName, Object languageObjet,BlockLanguageModel languageModel)
        {
            this.BlockName = blockName;
            this.LanguageModel = languageModel;
            this.LanguageModel.BlockName = blockName;
            this.BlockId = this.BlockName + "_1";

            this.LanguageModel.SetLanguage(languageObjet);
        }
        public BlockModel(object data_model,  string block_name, string block_id)
        {
            this.DataModel = data_model;
            
            BlockName = block_name;
            BlockId = block_id;
            this.LanguageModel = new BlockLanguageModel();
            this.LanguageModel.BlockName = block_name;
        }

        public BlockModel(object data_model, string block_name, string block_id, Object languageObjet)
        {
            this.DataModel = data_model;

            BlockName = block_name;
            BlockId = block_id;
            this.LanguageModel = new BlockLanguageModel();
            this.LanguageModel.BlockName = block_name;
            this.LanguageModel.SetLanguage(languageObjet);
        }
    }
}