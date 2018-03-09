using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using MicaData;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Format;

using Omu.AwesomeMvc;
using NPOI.SS.Util;
using NPOI.HSSF.Util;
using NPOI.XWPF.UserModel;

namespace CECHarmonization.Controllers
{
    public class ScriptExportController : Controller
    {

        DATA.MicaRepository ado = new DATA.MicaRepository();

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult ExportScriptToWord(string id)
        {
            IEnumerable<cohort_script_vw> scr = ado.GetCohortScriptByStudy(id);

            string txt;
            
            XWPFDocument doc = new XWPFDocument();
           

            XWPFParagraph p1 = doc.CreateParagraph();
            p1.Alignment = ParagraphAlignment.LEFT;
            
            XWPFRun head0 = p1.CreateRun();
            head0.SetText(string.Format("* Cohort Name: {0} ; ", scr.First().cohort_title));
            //head0.SetBold(true);
            head0.FontFamily = "Courier";
            head0.FontSize = 14;
            head0.AddCarriageReturn();

            XWPFRun head1 = p1.CreateRun();
            head1.SetText(string.Format("* Date Exported: {0} ; ", DateTime.Now.ToShortDateString()));
            //head1.SetBold(true);
            head1.FontFamily = "Courier";
            head1.FontSize = 14;
            head1.AddCarriageReturn();
            head1.AddCarriageReturn();

            XWPFRun head2 = p1.CreateRun();
            head2.SetText("***************************************************************");
            //head2.SetBold(true);
            head2.FontFamily = "Courier";
            head2.FontSize = 12;
            head2.AddCarriageReturn();
            head2.AddCarriageReturn();
            head2.AddCarriageReturn();


            //XWPFParagraph p1 = doc.CreateParagraph();
            //p1.Alignment = ParagraphAlignment.CENTER;
            //p1.BorderBottom = Borders.DOUBLE;
            //p1.BorderTop = Borders.DOUBLE;

            //p1.BorderRight = Borders.DOUBLE;
            //p1.BorderLeft = Borders.DOUBLE;
            //p1.BorderBetween = Borders.SINGLE;

            //p1.VerticalAlignment = TextAlignment.TOP;

            foreach (cohort_script_vw c in scr)
            {


                XWPFRun line2 = p1.CreateRun();
                line2.SetText("***************************************************************");
              //  line2.SetBold(false);
                line2.FontFamily = "Courier";
                line2.FontSize = 12;
                line2.AddCarriageReturn();

                XWPFRun com1 = p1.CreateRun();
                com1.SetText(string.Format("* TITLE: {0} ; ", c.title));
                //com1.SetBold(false);
                com1.FontFamily = "Courier";
                com1.FontSize = 12;
                com1.AddCarriageReturn();

                XWPFRun com2 = p1.CreateRun();
                com2.SetText(string.Format("* STATUS: {0} ; ", c.field_sva_status_value));
                //com2.SetBold(false);
                com2.FontFamily = "Courier";
                com2.FontSize = 12;
                com2.AddCarriageReturn();

                XWPFRun com3 = p1.CreateRun();
                txt = c.field_sva_comment_value.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                com3.SetText(string.Format("* COMMENT: {0} ; ", string.IsNullOrEmpty(c.field_sva_comment_value) ? "none" : txt));
                //com3.SetBold(false);
                com3.FontFamily = "Courier";
                com3.FontSize = 12;
                com3.AddCarriageReturn();
                com3.AddCarriageReturn();

                XWPFRun script0 = p1.CreateRun();
                script0.SetText("* SCRIPT: ");
                //script0.SetBold(false);
                script0.FontFamily = "Courier";
                script0.FontSize = 12;
                script0.AddCarriageReturn();

                XWPFRun script = p1.CreateRun();
                script.SetText(string.Format("{0} ", string.IsNullOrEmpty(c.field_sva_script_value) ? "* no script ;" : c.field_sva_script_value.Replace("\r\n", "").Replace("\r", "")));
                //script.SetBold(false);
                script.FontFamily = "Courier";
                script.FontSize = 12;
                script.AddCarriageReturn();
                script.AddCarriageReturn();
                script.AddCarriageReturn();

            }

            //XWPFParagraph p2 = doc.CreateParagraph();
            //p2.Alignment = ParagraphAlignment.RIGHT;

            ////BORDERS
            //p2.BorderBottom = Borders.DOUBLE;
            //p2.BorderTop = Borders.DOUBLE;
            //p2.BorderRight = Borders.DOUBLE;
            //p2.BorderLeft = Borders.DOUBLE;
            //p2.BorderBetween = Borders.SINGLE;

            //XWPFRun r2 = p2.CreateRun();
            //r2.SetText("jumped over the lazy dog");
            //r2.SetStrike(true);
            //r2.FontSize = 20;


            //XWPFRun r3 = p2.CreateRun();
            //r3.SetText("and went away");
            //r3.SetStrike(true);
            //r3.FontSize = 20;
            //r3.Subscript = VerticalAlign.SUPERSCRIPT;
            //r3.SetColor("FF0000");

            //XWPFParagraph p3 = doc.CreateParagraph();
            //p3.IsWordWrap = true;
            //p3.IsPageBreak = true;
            //p3.Alignment = ParagraphAlignment.BOTH;
            //p3.SpacingLineRule = LineSpacingRule.EXACT;
            //p3.IndentationFirstLine = 600;

            //XWPFRun r4 = p3.CreateRun();
            //r4.SetTextPosition(20);
            //r4.SetText("To be, or not to be: that is the question: "
            //        + "Whether 'tis nobler in the mind to suffer "
            //        + "The slings and arrows of outrageous fortune, "
            //        + "Or to take arms against a sea of troubles, "
            //        + "And by opposing end them? To die: to sleep; ");
            //r4.AddBreak(BreakType.PAGE);
            //r4.SetText("No more; and by a sleep to say we end "
            //        + "The heart-ache and the thousand natural shocks "
            //        + "That flesh is heir to, 'tis a consummation "
            //        + "Devoutly to be wish'd. To die, to sleep; "
            //        + "To sleep: perchance to dream: ay, there's the rub; "
            //        + ".......");
            //r4.IsItalic = true;
            ////This would imply that this break shall be treated as a simple line break, and break the line after that word:

            //XWPFRun r5 = p3.CreateRun();
            //r5.SetTextPosition(-10);
            //r5.SetText("For in that sleep of death what dreams may come");
            //r5.AddCarriageReturn();
            //r5.SetText("When we have shuffled off this mortal coil,"
            //        + "Must give us pause: there's the respect"
            //        + "That makes calamity of so long life;");
            //r5.AddBreak();
            //r5.SetText("For who would bear the whips and scorns of time,"
            //        + "The oppressor's wrong, the proud man's contumely,");

            //r5.AddBreak(BreakClear.ALL);
            //r5.SetText("The pangs of despised love, the law's delay,"
            //        + "The insolence of office and the spurns" + ".......");

            //FileStream out1 = new FileStream("simple.docx", FileMode.Create);
            //doc.Write(out1);
            //out1.Close();

            var stream = new MemoryStream();
            doc.Write(stream);
            stream.Close();

            return File(stream.ToArray(), "application/msword", "SimpleDoc.doc");
        }


    
    }

    
}