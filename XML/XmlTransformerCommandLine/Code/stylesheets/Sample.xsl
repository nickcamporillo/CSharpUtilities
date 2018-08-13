<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="xml"/>

    <xsl:template match="NewDataSet">
	   <DOC>
	      <LANGUAGE name="Minhast">
		     
			 <xsl:apply-templates select="Nouns"/>
			 <xsl:apply-templates select="Verbs"/>
			 <xsl:apply-templates select="Names"/>
			 
			 <xsl:apply-templates select="Adverb_Particles_and_Affixes"/>
			 <xsl:apply-templates select="Modals_and_Evidentials"/>
			 
			 <xsl:apply-templates select="Interjections"/>
			 <xsl:apply-templates select="Idioms"/>			 
			 
		  </LANGUAGE>
	   </DOC>	   
	</xsl:template>

	<!-- Nouns -->
    <xsl:template match="Nouns">
       <POS type="noun">
	       <ENTRY><xsl:value-of select="Noun"/></ENTRY>
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <FORMS type="gender">
		       <xsl:apply-templates select="Genders"/>
		   </FORMS>
		   <ENTRY_COMMENTS>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>
	
	<!-- Verbs -->
    <xsl:template match="Verbs">
       <POS type="verb">
	       <ENTRY><xsl:value-of select="Verb"/></ENTRY>
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <ENTRY_COMMENTS>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>

	<!-- Names -->
    <xsl:template match="Names">
       <POS type="noun" subtype="proper_noun" alias="Names">
	       <ENTRY><xsl:value-of select="People"/></ENTRY>  <!-- Yes, "Name" has been misnamed "People" -->
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <ENTRY_COMMENTS>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>	

	<!-- Interjections -->
    <xsl:template match="Interjections">
       <POS type="particle" subtype="interjection">
	       <ENTRY><xsl:value-of select="Interjection"/></ENTRY>
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <ENTRY_COMMENTS>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>

	<!-- Idioms -->
    <xsl:template match="Idioms">
       <POS type="idiom">
	       <ENTRY><xsl:value-of select="Idiom"/></ENTRY>
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <ENTRY_COMMENTS>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>		
	
	<!-- Adverbs, particles, and affixes -->
    <xsl:template match="Adverb_Particles_and_Affixes">
       <POS type="particle" alias="Adverb_Particles_and_Affixes">
	       <ENTRY><xsl:value-of select="Particle"/></ENTRY>
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <ENTRY_COMMENTS>
		       <COMMENT>
			       <VERBAL_AFFIX>
				       <xsl:value-of select="Verbal_Affix"/>
				   </VERBAL_AFFIX>
			   </COMMENT>
			   <COMMENT><WA_REQUIRED><xsl:value-of select="Wa_Construction_Required"/></WA_REQUIRED></COMMENT>
			   <COMMENT><WA_BINDING><xsl:value-of select="Postposed_Wa_Binding"/></WA_BINDING></COMMENT>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>

    <!-- Modals and Evidentials-->	
    <xsl:template match="Modals_and_Evidentials">
       <POS type="particle" alias="Modals_and_Evidentials">
	       <ENTRY><xsl:value-of select="Modal"/></ENTRY>
		   <MEANINGS><xsl:apply-templates select="Meaning"/></MEANINGS>
		   <DIALECTS><xsl:apply-templates select="Dialect"/></DIALECTS>
		   <ENTRY_COMMENTS>
		       <COMMENT><NEUTRALITY><xsl:value-of select="Neutral"/></NEUTRALITY></COMMENT>
			   <COMMENT><DUBITATIVE><xsl:value-of select="Dubitative"/></DUBITATIVE></COMMENT>
		       <xsl:apply-templates select="Additional_Notes"/>
			   <xsl:apply-templates select="Description"/>
		   </ENTRY_COMMENTS>
	   </POS>
    </xsl:template>
	
	<!-- Generic -->
    <xsl:template match="Meaning">
		<MEANING><xsl:value-of select="."/></MEANING>
    </xsl:template>

    <xsl:template match="Dialect">
	    <xsl:for-each select=".">
		   <DIALECT><xsl:value-of select="."/></DIALECT>
		</xsl:for-each>
    </xsl:template>
	
    <xsl:template match="Genders">
	    <xsl:for-each select="Gender">
		   <xsl:variable name="gender_val" select="."/>
		   <FORM type="Gender">
			   <xsl:value-of select="$gender_val"/>
		   </FORM>
		</xsl:for-each>
    </xsl:template>

    <xsl:template match="Additional_Notes">
		<COMMENT type="default" alias="Additional_Notes"><xsl:value-of select="."/>
		</COMMENT>
    </xsl:template>	
	
    <xsl:template match="Description">
		<COMMENT type="description" alias="description"><xsl:value-of select="."/>
		</COMMENT>
    </xsl:template>		
	
</xsl:stylesheet>	
