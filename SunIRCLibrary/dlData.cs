namespace SunIRCLibrary
{
    class dlData
    {
        public string dlId { get; set; }
        public string dlBot { get; set; }
        public string dlPack { get; set; }
        public int dlIndex { get; set; }
        public dlData()
        {
            //nadanoppes
        }

        public dlData(int dlIndex, string dlId, string dlBot, string dlPack)
        {
            this.dlIndex = dlIndex;
            this.dlBot = dlBot;
            this.dlId = dlId;
            this.dlPack = dlPack;
        }
    }
}
