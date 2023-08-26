namespace Model


#nowarn "3535"     // Static abstract members.

type IValueType<'T, 'U> =
    static abstract member create: 'U -> 'T
