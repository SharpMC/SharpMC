namespace fNbt {
    enum NbtParseState {
        AtStreamBeginning,
        AtCompoundBeginning,
        InCompound,
        AtCompoundEnd,
        AtListBeginning,
        InList,
        AtStreamEnd,
        Error
    }
}