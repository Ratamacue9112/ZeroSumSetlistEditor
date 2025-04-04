﻿using System;
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
        public byte[] GenerateDocument(List<SetlistDocumentRole> roles, bool printGeneral, Dictionary<Song, string> songs, Setlist setlist, SetlistSettings settings)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                bool isEmpty = true;
                if (printGeneral)
                {
                    isEmpty = false;
                    SetlistPage(container, null, -1, setlist, songs, settings);
                }
                for (int i = 0; i < roles.Count; i++)
                {
                    if (roles[i].ToPrint)
                    {
                        isEmpty = false;
                        SetlistPage(container, roles[i], i, setlist, songs, settings);
                    }
                }
                if (isEmpty)
                {
                    container.Page(page => { });
                }
            }).GeneratePdf();
        }

        private void SetlistPage(IDocumentContainer container, SetlistDocumentRole? role, int roleIndex, Setlist setlist, Dictionary<Song, string> songs, SetlistSettings settings)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                if (settings.FontFamily.IsInstalledFont())
                {
                    page.DefaultTextStyle(x => x.FontFamily(settings.FontFamily));
                }
                page.PageColor(settings.BackgroundColor.ToHex());

                page.Header()
                    .Text(text =>
                    {
                        if (settings.ShowVenue)
                        {
                            text.Span(setlist.Venue + Environment.NewLine)
                                .ExtraBold()
                                .FontColor(settings.HeaderColor.ToHex())
                                .FontSize(settings.HeaderSize);
                        }
                        if (settings.ShowDate)
                        {
                            text.Span(setlist.DateText + Environment.NewLine)
                                .SemiBold()
                                .FontColor(settings.HeaderColor.ToHex())
                                .FontSize(settings.HeaderSize / 10 * 9);
                        }
                        if (settings.ShowArtist)
                        {
                            text.Span(setlist.Artist)
                                .SemiBold()
                                .FontColor(settings.HeaderColor.ToHex())
                                .FontSize(settings.HeaderSize / 10 * 7);
                        }
                    });

                if (role != null)
                {
                    page.Footer()
                        .Text(role.Name)
                        .FontColor(settings.HeaderColor.ToHex());
                }

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x =>
                    {
                        int encoreCount = 0;
                        foreach(KeyValuePair<Song, string> song in songs)
                        {
                            if (song.Key.Name.StartsWith("--ENCORE"))
                            {
                                encoreCount++;
                                if (encoreCount > 1)
                                {
                                    x.Item()
                                        .Text("---- ENCORE " + encoreCount + " ----"
                                              + (song.Value == "" ? "" : (" (" + song.Value + ")")))
                                        .FontColor(settings.EncoreColor.ToHex())
                                        .FontSize(settings.EncoreSize)
                                        .ExtraBold();
                                }
                                else
                                {
                                    x.Item()
                                        .Text("---- ENCORE ----"
                                              + (song.Value == "" ? "" : (" (" + song.Value + ")")))
                                        .FontColor(settings.EncoreColor.ToHex())
                                        .FontSize(settings.EncoreSize)
                                        .ExtraBold();
                                }
                            }
                            else if (song.Key.Name == "--INTERMISSION--")
                            {
                                x.Item()
                                    .Text("---- INTERMISSION ----" 
                                          + (song.Value == "" ? "" : (" (" + song.Value + ")")))
                                    .FontColor(settings.IntermissionColor.ToHex())
                                    .FontSize(settings.IntermissionSize)
                                    .ExtraBold();
                            }
                            else 
                            {
                                x.Item().Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text((song.Key.ShortName == "" ? song.Key.Name.ToUpper() : song.Key.ShortName.ToUpper())
                                              + (song.Value == "" ? "" : (" (" + song.Value + ")")))
                                        .FontColor(settings.SongColor.ToHex())
                                        .FontSize(settings.SongSize);
                                    if (role != null && song.Key.Notes.Count > roleIndex)
                                    {
                                        row.RelativeItem()
                                            .Text(song.Key.Notes[roleIndex])
                                            .FontColor(settings.NoteColor.ToHex())
                                            .FontSize(settings.NoteSize);
                                    }           
                                });
                            }
                        }
                    });
            });
        }
    }
}
