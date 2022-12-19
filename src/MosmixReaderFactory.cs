namespace stefc.mosmix;

public static class MosmixReaderFactory {

    public static IMosmixReader CreateForKml(Stream data) => new MosmixXmlReader();

    public static IMosmixReader CreateForKmz(Stream data) => new MosmixZippedXmlReader();
}