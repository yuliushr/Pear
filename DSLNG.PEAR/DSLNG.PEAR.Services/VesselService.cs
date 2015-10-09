﻿using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Vessel;
using DSLNG.PEAR.Services.Responses.Vessel;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Data.Entity.Infrastructure;

namespace DSLNG.PEAR.Services
{
    public class VesselService : BaseService,IVesselService
    {
        public VesselService(IDataContext dataContext) : base(dataContext) { }

        public GetVesselResponse GetVessel(GetVesselRequest request)
        {
            return DataContext.Vessels
                .Include(x => x.Measurement)
                .FirstOrDefault(x => x.Id == request.Id)
                .MapTo<GetVesselResponse>();
        }

        public GetVesselsResponse GetVessels(GetVesselsRequest request)
        {
            if (request.OnlyCount)
            {
                return new GetVesselsResponse { Count = DataContext.Vessels.Count() };
            }
            else
            {
                var query = DataContext.Vessels
                    .Include(x => x.Measurement);
                if (!string.IsNullOrEmpty(request.Term)) {
                    query = query.Where(x => x.Name.Contains(request.Term));
                }
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetVesselsResponse
                {
                    Vessels = query.ToList().MapTo<GetVesselsResponse.VesselResponse>()
                };
            }
        }

        public SaveVesselResponse SaveVessel(SaveVesselRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var vessel = request.MapTo<Vessel>();
                    var measurement = new Measurement { Id = request.MeasurementId };
                    DataContext.Measurements.Attach(measurement);
                    vessel.Measurement = measurement;
                    DataContext.Vessels.Add(vessel);
                }
                else
                {
                    var vessel = DataContext.Vessels.FirstOrDefault(x => x.Id == request.Id);
                    if (vessel != null)
                    {
                        request.MapPropertiesToInstance<Vessel>(vessel);
                        var measurement = new Measurement { Id = request.MeasurementId };
                        DataContext.Measurements.Attach(measurement);
                        vessel.Measurement = measurement;
                    }
                }
                DataContext.SaveChanges();
                return new SaveVesselResponse
                {
                    IsSuccess = true,
                    Message = "Highlight has been saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveVesselResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }


        public DeleteVesselResponse Delete(DeleteVesselRequest request)
        {
            try
            {
                var vessel = new Vessel { Id = request.Id };
                DataContext.Vessels.Attach(vessel);
                DataContext.Vessels.Remove(vessel);
                DataContext.SaveChanges();
                return new DeleteVesselResponse
                {
                    IsSuccess = true,
                    Message = "You have been deleted this item successfully"
                };
            }
            catch (DbUpdateException e) {
                if (e.InnerException.InnerException.Message.Contains("dbo.VesselSchedules")) {
                    return new DeleteVesselResponse
                    {
                        IsSuccess = false,
                        Message = "This item is being used by Vessel Schedule"
                    };
                }
                return new DeleteVesselResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
            catch (InvalidOperationException)
            {
                return new DeleteVesselResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while trying to delete this item"
                };
            }
        }
    }
}
