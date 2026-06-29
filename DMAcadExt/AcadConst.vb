Option Explicit On
Option Strict On
Public Class AcadConst
	Public Const AcadBlockRefName As String = "AcDbBlockReference"
	Public Const AcadPolylineName As String = "AcDbPolyline"
	Public Const AcadLineName As String = "AcDbLine"
	Public Const AcadPointName As String = "AcDbPoint"
	Public Const AcadMPolygonName As String = "AcDbMPolygon"
	Public Const AcadHatchName As String = "AcDbHatch"
	Public Const AcadAttributeDefName As String = "AcDbAttributeDefinition"
   Public Const AcadTextName As String = "AcDbText"

	Public Const AcadFeatureLayerName As String = "AcMapBulkFeature"

   Public Const ScaleSysVarName As String = "USERR1"

	Public Const SnapModeSysVarName As String = "SNAPMODE"
	Public Const SnapUnitSysVarName As String = "SNAPUNIT"





	Public Const AcadLWPolylineName As String = "AcadLWPolyline"
	Public Const Acad2dPolylineName As String = "AcDb2dPolyline"
	Public Const AcadArcName As String = "AcDbArc"
	Public Const Acad2dVertexName As String = "AcDb2dVertex"


	Public Const TopoErrBlockRefOMark As String = "MAP_CLEAN_OMARK"
	Public Const TopoErrBlockRefRMark As String = "MAP_CLEAN_RMARK"
	Public Const TopoErrBlockRefSMark As String = "MAP_CLEAN_SMARK"
	Public Const TopoErrBlockRefTMark As String = "MAP_CLEAN_TMARK"
	Public Enum AcadValueType2018
		[Double] = 1040
		Double5000 = 5001
		[Short] = 1070
		Short5000 = 5003
		[String] = 410
		[String5000] = 5005
		[Integer] = 1071
		[Integer5000] = 5010
		Handle = 1005
		Undefined = 0
	End Enum
	Public Enum AcadValueType
		Undefined = 0 'RTNONE = 5000;  /* No result                   */
		[Double] = 1040  'RTREAL = 5001;  /* Real number              */
		' RTPOshort = 5002;   /* 2D poshort X and Y only              */
		[Short] = 1070 ' RTSHORT = 5003;     /* Short integer         */
		' RTANG = 5004;       /* Angle                                */             
		[String] = 410 ' RTSTR = 5005;       /* String               */
		' RTENAME = 5006;     /* Entity name                          */
		' RTPICKS = 5007;     /* Pick set                             */
		' RTORshort = 5008;   /* Orientation                          */
		' RT3DPOshort = 5009; /* 3D poshort - X, Y, and Z             */
		[Integer] = 1071 ' 5010     '/* Long integer                         */
		' RTVOID = 5014;      /* Blank symbol                         */
		' RTLB = 5016;        /* list begin                           */
		' RTLE = 5017;        /* list end                             */
		' RTDOTE = 5018;      /* dotted pair                          */
		' RTNIL = 5019;       /* nil                                  */
		' RTDXF0 = 5020;      /* DXF code 0 for ads_buildlist only    */
		' RTT = 5021;         /* T atom                               */
		' RTRESBUF = 5023;    /* resbuf                               */
		' RTMODELESS = 5027;  /* interrupted by modeless dialog       */

		Handle = 1005

		jujujj      ' // Error return code
		' RTNORM = 5100;      /* Request succeeded                    */
		' RTERROR = -5001;    /* Some other error                     */
		' RTCAN = -5002;      /* User cancelled request -- Ctl-C      */
		' RTREJ = -5003;      /* AutoCAD rejected request -- invalid  */
		' RTFAIL = -5004;     /* Link failure -- Lisp probably died   */
		' RTKWORD = -5005;    /* Keyword returned from getxxx() routine   */
		' RTINPUTTRUNCATED = -5008; /* Input didn't all fit in the buffer */


	End Enum

	Public Enum AcadValueType5000
		Undefined = 0 'RTNONE = 5000;  /* No result                   */
		[Double] = 5001  'RTREAL = 5001;  /* Real number              */
		' RTPOshort = 5002;   /* 2D poshort X and Y only              */
		[Short] = 5003 ' RTSHORT = 5003;     /* Short integer         */
		' RTANG = 5004;       /* Angle                                */             
		[String] = 5005 ' RTSTR = 5005;       /* String               */
		' RTENAME = 5006;     /* Entity name                          */
		' RTPICKS = 5007;     /* Pick set                             */
		' RTORshort = 5008;   /* Orientation                          */
		' RT3DPOshort = 5009; /* 3D poshort - X, Y, and Z             */
		[Integer] = 5010     '/* Long integer                         */
		' RTVOID = 5014;      /* Blank symbol                         */
		' RTLB = 5016;        /* list begin                           */
		' RTLE = 5017;        /* list end                             */
		' RTDOTE = 5018;      /* dotted pair                          */
		' RTNIL = 5019;       /* nil                                  */
		' RTDXF0 = 5020;      /* DXF code 0 for ads_buildlist only    */
		' RTT = 5021;         /* T atom                               */
		' RTRESBUF = 5023;    /* resbuf                               */
		' RTMODELESS = 5027;  /* interrupted by modeless dialog       */

		Handle = 1005

		' // Error return code
		' RTNORM = 5100;      /* Request succeeded                    */
		' RTERROR = -5001;    /* Some other error                     */
		' RTCAN = -5002;      /* User cancelled request -- Ctl-C      */
		' RTREJ = -5003;      /* AutoCAD rejected request -- invalid  */
		' RTFAIL = -5004;     /* Link failure -- Lisp probably died   */
		' RTKWORD = -5005;    /* Keyword returned from getxxx() routine   */
		' RTINPUTTRUNCATED = -5008; /* Input didn't all fit in the buffer */


	End Enum

End Class
