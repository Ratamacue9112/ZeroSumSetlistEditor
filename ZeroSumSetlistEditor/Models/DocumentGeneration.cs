using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using ZeroSumSetlistEditor.ViewModels;

namespace ZeroSumSetlistEditor.Models
{
    public class DocumentGeneration
    {
        public byte[] GenerateDocument(List<SetlistDocumentRole> roles, bool printGeneral, List<Song> songs, Setlist setlist)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                bool isEmpty = true;
                if (printGeneral)
                {
                    isEmpty = false;
                    SetlistPage(container, null, -1, setlist, songs);
                }
                for (int i = 0; i < roles.Count; i++)
                {
                    if (roles[i].ToPrint)
                    {
                        isEmpty = false;
                        SetlistPage(container, roles[i], i, setlist, songs);
                    }
                }
                if (isEmpty)
                {
                    container.Page(page => { });
                }
            }).GeneratePdf();
        }

        private void SetlistPage(IDocumentContainer container, SetlistDocumentRole? role, int roleIndex, Setlist setlist, List<Song> songs)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(14.5f));

                page.Header()
                    .Text(text => {
                        text.Span(setlist.Venue + Environment.NewLine)
                            .ExtraBold()
                            .FontSize(20);
                        text.Span(setlist.DateText)
                            .SemiBold()
                            .FontSize(18);

                    });

                if (role != null)
                {
                    page.Footer()
                        .Text(role.Name);
                }

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        int encoreCount = 0;
                        foreach(Song song in songs)
                        {
                            if (song.Name == "--ENCORE--")
                            {
                                encoreCount++;
                                if (encoreCount > 1)
                                {
                                    x.Item()
                                        .Text("----------------------------------- ENCORE " + encoreCount + " -----------------------------------")
                                        .ExtraBold();
                                }
                                else
                                {
                                    x.Item()
                                        .Text("------------------------------------ ENCORE ------------------------------------")
                                        .ExtraBold();
                                }
                            }
                            else if (song.Name == "--INTERMISSION--")
                            {
                                x.Item()
                                    .Text("-------------------------------- INTERMISSION --------------------------------")
                                    .ExtraBold();
                            }
                            else 
                            {
                                x.Item().Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text(song.Name.ToUpper());
                                    if (role != null)
                                    {
                                        row.RelativeItem()
                                            .Text(song.Notes[roleIndex]);
                                    }
                                });
                            }
                        }
                    });
            });
        }
    }
}
