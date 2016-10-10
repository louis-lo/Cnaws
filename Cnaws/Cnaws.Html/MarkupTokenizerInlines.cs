using System;

namespace Cnaws.Html
{
    internal enum HtmlState
    {
        DataState,
        CharacterReferenceInDataState,
        RCDATAState,
        CharacterReferenceInRCDATAState,
        RAWTEXTState,
        ScriptDataState,
        PLAINTEXTState,
        TagOpenState,
        EndTagOpenState,
        TagNameState,
        RCDATALessThanSignState,
        RCDATAEndTagOpenState,
        RCDATAEndTagNameState,
        RAWTEXTLessThanSignState,
        RAWTEXTEndTagOpenState,
        RAWTEXTEndTagNameState,
        ScriptDataLessThanSignState,
        ScriptDataEndTagOpenState,
        ScriptDataEndTagNameState,
        ScriptDataEscapeStartState,
        ScriptDataEscapeStartDashState,
        ScriptDataEscapedState,
        ScriptDataEscapedDashState,
        ScriptDataEscapedDashDashState,
        ScriptDataEscapedLessThanSignState,
        ScriptDataEscapedEndTagOpenState,
        ScriptDataEscapedEndTagNameState,
        ScriptDataDoubleEscapeStartState,
        ScriptDataDoubleEscapedState,
        ScriptDataDoubleEscapedDashState,
        ScriptDataDoubleEscapedDashDashState,
        ScriptDataDoubleEscapedLessThanSignState,
        ScriptDataDoubleEscapeEndState,
        BeforeAttributeNameState,
        AttributeNameState,
        AfterAttributeNameState,
        BeforeAttributeValueState,
        AttributeValueDoubleQuotedState,
        AttributeValueSingleQuotedState,
        AttributeValueUnquotedState,
        CharacterReferenceInAttributeValueState,
        AfterAttributeValueQuotedState,
        SelfClosingStartTagState,
        BogusCommentState,
        ContinueBogusCommentState, // Not in the HTML spec, used internally to track whether we started the bogus comment token.
        MarkupDeclarationOpenState,
        CommentStartState,
        CommentStartDashState,
        CommentState,
        CommentEndDashState,
        CommentEndState,
        CommentEndBangState,
        DOCTYPEState,
        BeforeDOCTYPENameState,
        DOCTYPENameState,
        AfterDOCTYPENameState,
        AfterDOCTYPEPublicKeywordState,
        BeforeDOCTYPEPublicIdentifierState,
        DOCTYPEPublicIdentifierDoubleQuotedState,
        DOCTYPEPublicIdentifierSingleQuotedState,
        AfterDOCTYPEPublicIdentifierState,
        BetweenDOCTYPEPublicAndSystemIdentifiersState,
        AfterDOCTYPESystemKeywordState,
        BeforeDOCTYPESystemIdentifierState,
        DOCTYPESystemIdentifierDoubleQuotedState,
        DOCTYPESystemIdentifierSingleQuotedState,
        AfterDOCTYPESystemIdentifierState,
        BogusDOCTYPEState,
        CDATASectionState,
        // These CDATA states are not in the HTML5 spec, but we use them internally.
        CDATASectionRightSquareBracketState,
        CDATASectionDoubleRightSquareBracketState,
    }

    internal delegate void HtmlStateHandler();

    internal static class MarkupTokenizerInlines
    {
        public static bool isTokenizerWhitespace(char character)
        {
            return character == ' ' || character == '\x0A' || character == '\x09' || character == '\x0C';
        }
        public static void BEGIN_STATE(HtmlState stateName, HtmlStateHandler handler)
        {

        }
    }
}
