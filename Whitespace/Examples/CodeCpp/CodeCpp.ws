//  [ github.com/3F	 	Whitespace-Example1 (wspace-v0.3)		
 
... 	
/**  	
* To work with an interval:	 	    
*	,_________,
*{word} ... {word}   			  	 
*/	
udiff_t interval();    		  			
	
/** forward to next SPLIT-block */	  		 
	
bool nextBlock(tstring::const_iterator& it, bool delta =true);	     
	
private:     		    
	
/** this is target sequence */				   
	
tstring _text;    		  		
	
/** the main pattern */ 	   		 
	
tstring _filter; 


inline tstring _lowercase(tstring str) throw()
{
	transform(str.begin(), str.end(), str.begin(), towlower);
	return str;
};
...



// - - - - - - - - - - - - -
// https://ideone.com/G1uKlw
// -_*